using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Inventory
{
    [Serializable]
    public class InventoryItem
    {
        [OdinSerialize]
        [HideInInspector]
        public InventoryComponent InventoryComponent { get; set; }

        [OdinSerialize]
        [TableColumnWidth(57, false)]
        [PreviewField(ObjectFieldAlignment.Center)]
        [OnValueChanged("OnProductModified")]
        Sprite Icon { get; set; }

        [OdinSerialize]
        [OnValueChanged("OnProductChanged")]
        [VerticalGroup("Product")]
        [HideLabel]
        public Product Product { get; set; }

        [OdinSerialize]
        [OnValueChanged("OnProductModified")]
        [VerticalGroup("Product")]
        [HideLabel]
        string Name { get; set; }

        [OdinSerialize]
        public int Supply { get; set; }

        [Button]
        [VerticalGroup("Actions")]
        void Unregister()
        {
            InventoryComponent.UnregisterProduct(Product);
        }

        public InventoryItem(Product product, InventoryComponent inventoryComponent, int supply = 0)
        {
            Product = product;
            InventoryComponent = inventoryComponent;
            if (supply != 0) Supply = supply;

            OnProductChanged();
        }

        void OnProductChanged()
        {
            if (!Product) return;
            Icon = Product.Icon;
            Name = Product.Name;
        }

        void OnProductModified()
        {
            if (!Product) return;
            Product.Icon = Icon;
            Product.Name = Name;
        }

        public static implicit operator Product(InventoryItem inventoryItem) => inventoryItem.Product;
    }
}