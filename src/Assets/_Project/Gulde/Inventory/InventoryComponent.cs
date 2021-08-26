using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Economy;
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
        [TableList]
        [OnInspectorInit("OnInventoryInit")]
        [OnCollectionChanged("OnInventoryChanged")]
        public List<InventoryItem> Items { get; set; } = new List<InventoryItem>();

        [OdinSerialize]
        public int Slots { get; set; }

        [ShowInInspector]
        public int FreeSlots => Slots - Items.Count + Items.Count(e => e.Item == null);

        [OdinSerialize]
        bool UnregisterWhenEmpty { get; set; }

        [ShowInInspector]
        public bool IsFull => Items != null && FreeSlots <= 0;

        [ShowInInspector]
        public bool IsEmpty => Items != null && FreeSlots == Slots;

        public event EventHandler<ItemEventArgs> Added;
        public event EventHandler<ItemEventArgs> Removed;

        public bool IsRegistered(Item item) => Items.Any(e => e.Item == item);

        public bool HasProductInStock(Item item, int amount = 1) => IsRegistered(item) && Items.Find(e => e.Item == item).Supply >= amount;

        public bool CanAddItem(Item item) => IsRegistered(item) || !IsFull;

        InventoryItem GetInventoryItem(Object product) => Items.Find(e => e.Item == product);

        public int GetSupply(Item item) =>
            IsRegistered(item) ? Items.Find(e => e.Item == item).Supply : 0;

        public void Register(Item item)
        {
            Items.RemoveAll(e => !e.Item);

            if (IsFull) return;
            if (IsRegistered(item)) return;

            Items.Add(new InventoryItem(item, this));
        }

        public void UnregisterProduct(Item item)
        {
            if (!IsRegistered(item)) return;

            var inventoryItem = Items.Find(e => e.Item == item);
            Items.Remove(inventoryItem);
        }

        public void Add(Item item, int amount = 1)
        {
            if (!CanAddItem(item)) return;

            Register(item);
            Items.Find(e => e.Item == item).Supply += amount;
            Added?.Invoke(this, new ItemEventArgs(item, amount));
        }

        public void Remove(Item item, int amount = 1)
        {
            if (!IsRegistered(item)) return;

            var inventoryItem = GetInventoryItem(item);
            inventoryItem.Supply = Mathf.Max(inventoryItem.Supply - amount, 0);

            if (UnregisterWhenEmpty) UnregisterProduct(item);
            Removed?.Invoke(this, new ItemEventArgs(item, amount));
        }

        public void RemoveResources(Recipe recipe)
        {
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

        public void AddResources(Recipe recipe)
        {
            foreach (var pair in recipe.Resources)
            {
                var resource = pair.Key;
                var amount = pair.Value;

                for (var i = 0; i < amount; i++) Add(resource);
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