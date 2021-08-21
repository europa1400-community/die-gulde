using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
        public List<InventoryItem> Inventory { get; set; } = new List<InventoryItem>();

        [OdinSerialize]
        int Slots { get; set; }

        [OdinSerialize]
        bool UnregisterWhenEmpty { get; set; }

        [ShowInInspector]
        public bool IsFull => Inventory != null && Inventory.Count >= Slots;

        public bool IsRegistered(Item item) => Inventory.Any(e => e.Item == item);

        public bool HasProductInStock(Item item) => IsRegistered(item) && Inventory.Find(e => e.Item == item).Supply > 0;

        public bool CanAddProduct(Item item) => IsRegistered(item) || !IsFull;

        InventoryItem GetInventoryItem(Object product) => Inventory.Find(e => e.Item == product);

        public int GetSupply(Item item) =>
            IsRegistered(item) ? Inventory.Find(e => e.Item == item).Supply : 0;

        public void Register(Item item)
        {
            if (IsFull) return;
            if (IsRegistered(item)) return;

            Inventory.Add(new InventoryItem(item, this));
        }

        public void UnregisterProduct(Item item)
        {
            if (!IsRegistered(item)) return;

            var inventoryItem = Inventory.Find(e => e.Item == item);
            Inventory.Remove(inventoryItem);
        }

        public void Add(Item item)
        {
            if (!CanAddProduct(item)) return;

            Register(item);
            Inventory.Find(e => e.Item == item).Supply += 1;
        }

        public void Remove(Item item)
        {
            if (!IsRegistered(item)) return;

            var inventoryItem = GetInventoryItem(item);
            inventoryItem.Supply = Mathf.Max(inventoryItem.Supply - 1, 0);

            if (UnregisterWhenEmpty) UnregisterProduct(item);
        }

        #region OdinInspector

        void OnInventoryInit()
        {
            Inventory ??= new List<InventoryItem>();

            foreach (var inventoryItem in Inventory)
            {
                inventoryItem.InventoryComponent = this;
            }
        }

        void OnInventoryChanged()
        {
            foreach (var inventoryItem in Inventory)
            {
                inventoryItem.InventoryComponent = this;
            }
        }

        #endregion

    }
}