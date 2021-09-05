using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Economy;
using MonoLogger.Runtime;
using Gulde.Production;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gulde.Inventory
{
    public class InventoryComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        public int Slots { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        bool UnregisterWhenEmpty { get; set; }

        [OdinSerialize]
        [BoxGroup("Info")]
        [TableList]
        [OnInspectorInit("OnInventoryInit")]
        [OnCollectionChanged("OnInventoryChanged")]
        public List<InventoryItem> Items { get; set; } = new List<InventoryItem>();

        [ShowInInspector]
        [BoxGroup("Info")]
        public int FreeSlots => Slots - Items.Count + Items.Count(e => e.Item == null);

        [ShowInInspector]
        [BoxGroup("Info")]
        public bool IsFull => Items != null && FreeSlots <= 0;

        [ShowInInspector]
        [BoxGroup("Info")]
        public bool IsEmpty => Items != null && FreeSlots == Slots;

        public event EventHandler<ItemEventArgs> Added;
        public event EventHandler<ItemEventArgs> Removed;

        public bool IsRegistered(Item item) => Items.Any(e => e.Item == item);

        public bool HasProductInStock(Item item, int amount = 1) => IsRegistered(item) && Items.Find(e => e.Item == item).Supply >= amount;

        public bool CanAddItem(Item item) => IsRegistered(item) || !IsFull;

        InventoryItem GetInventoryItem(Object product) => Items.Find(e => e.Item == product);

        public int GetSupply(Item item) =>
            IsRegistered(item) ? Items.Find(e => e.Item == item).Supply : 0;

        void Awake()
        {
            this.Log("Inventory intializing");
        }

        public void Register(Item item)
        {
            this.Log($"Inventory registering {item}");

            Items.RemoveAll(e => !e.Item);

            if (IsFull) return;
            if (IsRegistered(item)) return;

            Items.Add(new InventoryItem(item, this));
        }

        public void UnregisterProduct(Item item)
        {
            this.Log($"Inventory unregistering {item}");

            if (!IsRegistered(item)) return;

            var inventoryItem = Items.Find(e => e.Item == item);
            Items.Remove(inventoryItem);
        }

        public void Add(Item item, int amount = 1)
        {
            this.Log($"Inventory adding {amount} {item}");

            if (!CanAddItem(item)) return;

            Register(item);
            Items.Find(e => e.Item == item).Supply += amount;
            Added?.Invoke(this, new ItemEventArgs(item, amount));
        }

        public void Remove(Item item, int amount = 1)
        {
            this.Log($"Inventory removing {amount} {item}");

            if (!IsRegistered(item)) return;

            var inventoryItem = GetInventoryItem(item);
            inventoryItem.Supply = Mathf.Max(inventoryItem.Supply - amount, 0);

            if (UnregisterWhenEmpty) UnregisterProduct(item);
            Removed?.Invoke(this, new ItemEventArgs(item, amount));
        }

        public void AddResources(Recipe recipe)
        {
            this.Log($"Inventory adding resources for {recipe}");

            foreach (var pair in recipe.Resources)
            {
                var resource = pair.Key;
                var amount = pair.Value;

                for (var i = 0; i < amount; i++) Add(resource);
            }
        }

        public void RemoveResources(Recipe recipe)
        {
            this.Log($"Inventory removing resources for {recipe}");

            foreach (var pair in recipe.Resources)
            {
                var resource = pair.Key;
                var amount = pair.Value;

                for (var i = 0; i < amount; i++)
                {
                    Remove(resource);
                }
            }
        }

        #region OdinInspector

        void OnInventoryInit()
        {
            Items ??= new List<InventoryItem>();

            foreach (var inventoryItem in Items)
            {
                inventoryItem.InventoryComponent = this;
            }
        }

        void OnInventoryChanged()
        {
            foreach (var inventoryItem in Items)
            {
                inventoryItem.InventoryComponent = this;
            }
        }

        #endregion

    }
}