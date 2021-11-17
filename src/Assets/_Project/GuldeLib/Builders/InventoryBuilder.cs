using System.Collections.Generic;
using GuldeLib.Economy;
using GuldeLib.Inventories;

namespace GuldeLib.Builders
{
    public class InventoryBuilder : Builder<Inventory>
    {
        public InventoryBuilder WithSlots(int slots)
        {
            Object.Slots = slots;
            return this;
        }

        public InventoryBuilder WithUnregisterWhenEmpty(bool unregisterWhenEmpty)
        {
            Object.UnregisterWhenEmpty = unregisterWhenEmpty;
            return this;
        }

        public InventoryBuilder WithDisallowUnregister(bool disallowUnregister)
        {
            Object.DisallowUnregister = disallowUnregister;
            return this;
        }

        public InventoryBuilder WithItems(Dictionary<Item, int> items)
        {
            Object.Items = items;
            return this;
        }
    }
}