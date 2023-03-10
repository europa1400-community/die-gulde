using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Gulde.Core
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
                itemFlags.Any() && !item.itemFlags.Any(itemFlags.Contains))
            {
                return false;
            }
            
            slot.item = item;
            slot.count = 0;
            return true;
        }

        public bool Unregister(InventorySlot slot)
        {
            if (!slot.item ||
                slot.count > 0)
            {
                return false;
            }
            
            slot.item = null;
            slot.count = 0;
            return true;
        }

        public bool Add(Item item, InventorySlot slot, int count = 1)
        {
            if (slot.item != item ||
                slot.count + count > stackSize)
            {
                return false;
            }

            slot.count += count;
            return true;
        }

        public bool Remove(Item item, InventorySlot slot, int count = 1)
        {
            if (slot.item != item ||
                slot.count - count < 0)
            {
                return false;
            }

            slot.count -= count;
            return true;
        }
    }
}