using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Economy;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
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

        public bool HasProduct(Item item) => Inventory.Any(e => e.Item == item);

        public bool HasProductInStock(Item item) => HasProduct(item) && Inventory.Find(e => e.Item == item).Supply > 0;

        public bool CanAddProduct(Item item) => HasProduct(item) || !IsFull;

        InventoryItem GetInventoryItem(Object product) => Inventory.Find(e => e.Item == product);

        public int GetSupply(Item item) =>
            HasProduct(item) ? Inventory.Find(e => e.Item == item).Supply : 0;

        public void Register(Item item)
        {
            if (IsFull) return;
            if (HasProduct(item)) return;

            Inventory.Add(new InventoryItem(item, this));
        }

        public void UnregisterProduct(Item item)
        {
            if (!HasProduct(item)) return;

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
            if (!HasProduct(item)) return;

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