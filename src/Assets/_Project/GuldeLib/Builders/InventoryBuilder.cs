using System.Collections.Generic;
using GuldeLib.Economy;
using GuldeLib.Inventories;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class InventoryBuilder : Builder<Inventory>
    {
        public InventoryBuilder WithMaxCapacity(int maxCapacity)
        {
            Object.MaxCapacity = maxCapacity;
            return this;
        }

        public InventoryBuilder WithMaxSlots(int maxSlots)
        {
            Object.MaxSlots = maxSlots;
            return this;
        }

        public InventoryBuilder WithSlots(List<InventoryComponent.Slot> slots)
        {
            Object.Slots = slots;
            return this;
        }

        public InventoryBuilder WithAllowAutoUnregister(bool allowAutoUnregister)
        {
            Object.AllowAutoUnregister = allowAutoUnregister;
            return this;
        }
    }
}