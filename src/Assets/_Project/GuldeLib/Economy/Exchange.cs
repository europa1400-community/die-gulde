using System.Collections.Generic;
using GuldeLib.Economy;
using GuldeLib.Generators;
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
    [CreateAssetMenu(menuName = "Economy/Exchange")]
    public class Exchange : TypeObject<Exchange>
    {
        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableNaming Naming { get; set; }

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableInventory Inventory { get; set; } = new GeneratableInventory();

        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableInventory ProductInventory { get; set; }

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