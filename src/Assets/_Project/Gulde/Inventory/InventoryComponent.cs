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

        public bool HasProduct(Product product) => Inventory.Any(e => e.Product == product);

        public bool HasProductInStock(Product product) => HasProduct(product) && Inventory.Find(e => e.Product == product).Supply > 0;

        public bool CanAddProduct(Product product) => HasProduct(product) || !IsFull;

        InventoryItem GetInventoryItem(Object product) => Inventory.Find(e => e.Product == product);

        public int GetSupply(Product product) =>
            HasProduct(product) ? Inventory.Find(e => e.Product == product).Supply : 0;

        public void RegisterProduct(Product product)
        {
            if (IsFull) return;
            if (HasProduct(product)) return;

            Inventory.Add(new InventoryItem(product, this));
        }

        public void UnregisterProduct(Product product)
        {
            if (!HasProduct(product)) return;

            var inventoryItem = Inventory.Find(e => e.Product == product);
            Inventory.Remove(inventoryItem);
        }

        public void AddProduct(Product product)
        {
            if (!CanAddProduct(product)) return;

            RegisterProduct(product);
            Inventory.Find(e => e.Product == product).Supply += 1;
        }

        public void RemoveProduct(Product product)
        {
            if (!HasProduct(product)) return;

            var inventoryItem = GetInventoryItem(product);
            inventoryItem.Supply = Mathf.Max(inventoryItem.Supply - 1, 0);

            if (UnregisterWhenEmpty) UnregisterProduct(product);
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