using System.Collections.Generic;
using GuldeLib.Economy;
using UnityEngine;
using GuldeLib.Inventories;
using GuldeLib.TypeObjects;

namespace GuldeLib.Factories
{
    public class InventoryFactory : Factory<Inventory>
    {
        public InventoryFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject) { }

        public override GameObject Create(Inventory inventory)
        {
            var inventoryComponent = GameObject.AddComponent<InventoryComponent>();

            inventoryComponent.Items = new Dictionary<Item, int>(inventory.Items);
            inventoryComponent.Slots = inventory.Slots;
            inventoryComponent.DisallowUnregister = inventory.DisallowUnregister;
            inventoryComponent.UnregisterWhenEmpty = inventory.UnregisterWhenEmpty;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}