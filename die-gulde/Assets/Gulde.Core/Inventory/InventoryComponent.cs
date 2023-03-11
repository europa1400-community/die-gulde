using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gulde.Core.Inventory
{
    public class InventoryComponent : MonoBehaviour
    {
        /// <summary>
        /// The maximum amount of an item a slot can hold
        /// </summary>
        [SerializeField]
        public int stackSize;

        /// <summary>
        /// The amount of slots for item stacks the inventory has
        /// </summary>
        [SerializeField]
        public int slotCount;

        /// <summary>
        /// The list of slots for item stacks the inventory has 
        /// </summary>
        [SerializeField]
        public List<InventorySlot> slots = new();
        
        /// <summary>
        /// Items must have at least one of these ItemFlags to be allowed in this inventory
        /// </summary>
        [SerializeField]
        public List<ItemFlag> itemFlags = new();

        public bool Register(Item item, InventorySlot slot)
        {
            if (slot.item ||
                !IsAllowed(item))
            {
                return false;
            }
            
            slot.item = item;
            slot.supply = 0;
            return true;
        }

        public bool Unregister(InventorySlot slot)
        {
            if (!slot.item ||
                slot.supply > 0)
            {
                return false;
            }
            
            slot.item = null;
            slot.supply = 0;
            return true;
        }

        public bool Add(Item item, InventorySlot slot, int count = 1)
        {
            if (slot is null ||
                slot.item != item ||
                slot.supply + count > stackSize)
            {
                return false;
            }

            slot.supply += count;
            return true;
        }

        public bool Remove(Item item, InventorySlot slot, int count = 1)
        {
            if (slot is null ||
                slot.item != item ||
                slot.supply - count < 0)
            {
                return false;
            }
            
            slot.supply -= count;
            return true;
        }

        public bool Add(Item item, int count = 1)
        {
            if (!HasSpace(item, count))
            {
                return false;
            }

            var slotsWithItem = slots
                .Where(e => e.item == item)
                .OrderBy(e => e.supply)
                .ToList();
            
            var remainingCount = count;
            
            foreach (var slot in slotsWithItem)
            {
                if (remainingCount <= 0)
                {
                    return true;
                }
                
                var addCount = Mathf.Min(remainingCount, stackSize - slot.supply);
                Add(item, slot, addCount);
                remainingCount -= addCount;
            }
            
            var slotsWithoutItem = slots
                .Where(e => e.item is null)
                .OrderByDescending(e => e.supply)
                .ToList();

            foreach (var slot in slotsWithoutItem)
            {
                if (remainingCount <= 0)
                {
                    return true;
                }
                
                var addCount = Mathf.Min(remainingCount, stackSize);
                Register(item, slot);
                Add(item, slot, addCount);
                remainingCount -= addCount;
            }
            
            return remainingCount <= 0;
        }

        public bool Remove(Item item, int count = 1)
        {
            if (!HasItem(item, count))
            {
                return false;
            }
            
            var slotsWithItem = slots
                .Where(e => e.item == item)
                .OrderBy(e => e.supply)
                .ToList();

            var remainingCount = count;
            
            foreach (var slot in slotsWithItem)
            {
                if (remainingCount <= 0)
                {
                    return true;
                }
                
                var removeCount = Mathf.Min(remainingCount, slot.supply);
                Remove(item, slot, removeCount);
                remainingCount -= removeCount;
            }
            
            return remainingCount <= 0;
        }

        public bool HasSpace(Item item, int count = 1)
        {
            var totalSpace = slots
                .Where(e => e.item == item || e.item is null)
                .Sum(e => stackSize - e.supply);
            
            return totalSpace >= count;
        }

        public bool HasItem(Item item, int count = 1)
        {
            var totalSupply = slots
                .Where(e => e.item == item)
                .Sum(e => e.supply);
            
            return totalSupply >= count;
        }
        
        public bool IsAllowed(Item item) => 
            !itemFlags.Any() || item.itemFlags.Any(itemFlags.Contains);
    }
}