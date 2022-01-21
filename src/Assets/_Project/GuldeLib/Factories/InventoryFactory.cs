using System.Collections.Generic;
using GuldeLib.Economy;
using UnityEngine;
using GuldeLib.Inventories;
using GuldeLib.TypeObjects;

namespace GuldeLib.Factories
{
    public class InventoryFactory : Factory<Inventory, InventoryComponent>
    {
        public InventoryFactory(Inventory inventory, GameObject gameObject = null, GameObject parentObject = null, bool allowMultiple = false) : base(inventory, gameObject, parentObject, allowMultiple) { }

        public override InventoryComponent Create()
        {
            Component.Items = new Dictionary<Item, int>(TypeObject.Items);
            Component.Slots = TypeObject.Slots;
            Component.DisallowUnregister = TypeObject.DisallowUnregister;
            Component.UnregisterWhenEmpty = TypeObject.UnregisterWhenEmpty;

            return Component;
        }
    }
}