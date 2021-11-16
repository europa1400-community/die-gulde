using System.Collections.Generic;
using GuldeLib.Economy;
using GuldeLib.Inventories;
using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Economy
{
    /// <summary>
    /// Contains configuration data required to build objects containing <see cref = "ExchangeComponent">ExchangeComponents</see>.
    /// </summary>
    public class Exchange : SerializedScriptableObject
    {
        [Optional]
        [OdinSerialize]
        public Naming Naming { get; set; }

        [Required]
        [OdinSerialize]
        public Inventory Inventory { get; set; }

        [Optional]
        [OdinSerialize]
        public Inventory ProductInventory { get; set; }

        /// <inheritdoc cref="ExchangeComponent.IsPurchasing"/>
        [Required]
        [OdinSerialize]
        public bool IsPurchasing { get; set; } = true;

        /// <inheritdoc cref="ExchangeComponent.IsSelling"/>
        [Required]
        [OdinSerialize]
        public bool IsSelling { get; set; } = true;
    }
}