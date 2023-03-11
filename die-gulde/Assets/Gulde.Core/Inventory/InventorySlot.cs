using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gulde.Core.Inventory
{
    /// <summary>
    /// A slot for an item stack in an inventory
    /// </summary>
    [Serializable]
    public class InventorySlot
    {
        /// <summary>
        /// The item that the slot holds
        /// </summary>
        [SerializeField]
        public Item item;
        
        /// <summary>
        /// The amount of the item that the slot holds
        /// </summary>
        [SerializeField]
        public int supply;
    }
}