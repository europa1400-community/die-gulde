using GuldeLib.Economy;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    /// <summary>
    /// Contains configuration data required to build objects containing <see cref = "ExchangeComponent">ExchangeComponents</see>.
    /// </summary>
    [CreateAssetMenu(menuName = "Economy/Exchange")]
    public class Exchange : TypeObject<Exchange>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableInventory Inventory { get; set; } = new GeneratableInventory();

        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableInventory ProductInventory { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref = "ExchangeComponent">ExchangeComponent</see> will automatically purchase items.
        /// </summary>
        /// <remarks>
        /// This is used to prevent item sales to ExchangeComponents that don't generally accept anything.
        /// An example for this would be companies and player inventories.
        /// </remarks>
        [Required]
        [Setting]
        [OdinSerialize]
        public bool IsPurchasing { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the <see cref = "ExchangeComponent">ExchangeComponent</see> will automatically sell items.
        /// </summary>
        /// <remarks>
        /// This is used to prevent item purchases from ExchangeComponents that don't generally sell anything.
        /// An example for this would be companies and player inventories.
        /// </remarks>
        [Required]
        [Setting]
        [OdinSerialize]
        public bool IsSelling { get; set; } = true;

        public override bool SupportsNaming => true;
    }
}