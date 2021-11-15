using System.Collections.Generic;
using GuldeLib.Economy;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Economy
{
    /// <summary>
    /// Contains configuration data required to build objects containing <see cref = "ExchangeComponent">ExchangeComponents</see>.
    /// </summary>
    public class Exchange : SerializedScriptableObject
    {
        /// <summary>
        /// Gets or sets the dictionary mapping items to item amount the exchange should be built with.
        /// For further reference see <see cref = "GuldeLib.Inventory.InventoryComponent.Items">InventoryComponent.Items</see>.
        /// </summary>
        [ShowInInspector]
        public Dictionary<Item, int> StartItems { get; set; } = new Dictionary<Item, int>();

        /// <summary>
        /// Gets or sets the amount of inventory slots the exchange should be built with.
        /// For further reference see <see cref = "GuldeLib.Inventory.InventoryComponent.Slots">InventoryComponent.Slots</see>.
        /// </summary>
        [ShowInInspector]
        public int Slots { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets whether the exchange should be able to accept items.
        /// For further reference see <see cref = "ExchangeComponent.IsAccepting">ExchangeComponent.IsAccepting</see>.
        /// </summary>
        [ShowInInspector]
        public bool IsAccepting { get; set; } = true;
    }
}