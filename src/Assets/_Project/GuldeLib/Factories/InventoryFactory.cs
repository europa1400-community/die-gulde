using System.Collections.Generic;
using GuldeLib.Economy;
using UnityEngine;
using GuldeLib.Inventories;
using GuldeLib.TypeObjects;

namespace GuldeLib.Factories
{
    public class InventoryFactory : Factory<Inventory, InventoryComponent>
    {
        public InventoryFactory(Inventory inventory, GameObject gameObject = null, GameObject parentObject = null, bool allowMultiple = false, bool startInactive = false) : base(inventory, gameObject, parentObject, allowMultiple, startInactive) { }

        public override InventoryComponent Create()
        {
            Component.MaxCapacity = TypeObject.MaxCapacity;
            Component.MaxSlots = TypeObject.MaxSlots;
            Component.Slots = new List<InventoryComponent.Slot>(TypeObject.Slots);
            Component.AllowAutoUnregister = TypeObject.AllowAutoUnregister;

            return Component;
        }
    }
}