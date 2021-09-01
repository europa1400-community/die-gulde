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
        public Item Item { get; set; }

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
            InventoryComponent.UnregisterProduct(Item);
        }

        public InventoryItem(Item item, InventoryComponent inventoryComponent, int supply = 0)
        {
            Item = item;
            InventoryComponent = inventoryComponent;
            if (supply != 0) Supply = supply;

            OnProductChanged();
        }

        void OnProductChanged()
        {
            if (!Item) return;
            Icon = Item.Icon;
            Name = Item.Name;
        }

        void OnProductModified()
        {
            if (!Item) return;
            // Item.Icon = Icon;
            // Item.Name = Name;
        }

        public static implicit operator Item(InventoryItem inventoryItem) => inventoryItem.Item;
    }
}