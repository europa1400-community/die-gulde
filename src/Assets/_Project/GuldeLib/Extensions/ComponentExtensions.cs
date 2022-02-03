using System.Linq;
using GuldeLib.Inventories;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Extensions
{
    public static class ComponentExtensions
    {
        public static InventoryComponent GetInventory(this Component component, Item.ItemType type) =>
            component.GetComponents<InventoryComponent>().ElementAtOrDefault((int)type);
    }
}