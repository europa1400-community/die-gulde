using System.Collections.Generic;
using GuldeLib.Economy;
using UnityEngine;
using GuldeLib.Inventories;
using GuldeLib.TypeObjects;

namespace GuldeLib.Factories
{
    public class InventoryFactory : Factory<Inventory, InventoryComponent>
    {
        public InventoryFactory(GameObject gameObject = null, GameObject parentObject = null, bool allowMultiple = false) : base(gameObject, parentObject, allowMultiple) { }

        public override InventoryComponent Create(Inventory inventory)
        {
            Component.Items = new Dictionary<Item, int>(inventory.Items);
            Component.Slots = inventory.Slots;
            Component.DisallowUnregister = inventory.DisallowUnregister;
            Component.UnregisterWhenEmpty = inventory.UnregisterWhenEmpty;

            return Component;
        }
    }
}