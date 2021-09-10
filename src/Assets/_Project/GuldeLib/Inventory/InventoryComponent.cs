using System;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Economy;
using GuldeLib.Production;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GuldeLib.Inventory
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
        [BoxGroup("Settings")]
        bool DisallowUnregister { get; set; }

        [OdinSerialize]
        [BoxGroup("Info")]
        [TableList]
        public Dictionary<Item, int> Items { get; } = new Dictionary<Item, int>();

        [ShowInInspector]
        [BoxGroup("Info")]
        public int FreeSlots => Slots - Items.Count + Items.Count(e => e.Key == null);

        [ShowInInspector]
        [BoxGroup("Info")]
        public Item EmptySlot => Items.FirstOrDefault(pair => pair.Value <= 0).Key;

        [ShowInInspector]
        [BoxGroup("Info")]
        public bool IsFull => Items != null && FreeSlots <= 0;

        [ShowInInspector]
        [BoxGroup("Info")]
        public bool IsEmpty => Items != null && FreeSlots == Slots;

        public event EventHandler<ItemEventArgs> Added;
        public event EventHandler<ItemEventArgs> Removed;

        public bool IsRegistered(Item item) => Items.Any(e => e.Key == item);

        public bool HasProductInStock(Item item, int amount = 1) => IsRegistered(item) && Items[item] >= amount;

        public bool CanAddItem(Item item) => IsRegistered(item) || !IsFull || !DisallowUnregister && EmptySlot;

        public int GetSupply(Item item) =>
            IsRegistered(item) ? Items.First(pair => pair.Key == item).Value : 0;

        void Awake()
        {
            this.Log("Inventory intializing");
        }

        public void Register(Item item)
        {
            this.Log($"Inventory registering {item}");

            if (IsFull)
            {
                if (!DisallowUnregister && EmptySlot)
                {
                    Unregister(EmptySlot);
                }
                else return;
            }
            if (IsRegistered(item)) return;

            Items.Add(item, 0);
        }

        public void Unregister(Item item)
        {
            this.Log($"Inventory unregistering {item}");

            if (!IsRegistered(item)) return;

            Items.Remove(item);
        }

        public void Add(Item item, int amount = 1)
        {
            this.Log($"Inventory adding {amount} {item}");

            if (!CanAddItem(item))
            {
                this.Log($"Inventory can not add {item}", LogType.Warning);
                return;
            }

            Register(item);

            Items[item] += amount;
            Added?.Invoke(this, new ItemEventArgs(item, amount));
        }

        public void Remove(Item item, int amount = 1)
        {
            this.Log($"Inventory removing {amount} {item}");

            if (!IsRegistered(item)) return;

            Items[item] = Mathf.Max(Items[item] - amount, 0);

            if (UnregisterWhenEmpty) Unregister(item);
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
    }
}