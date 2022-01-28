using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Companies/Master")]
    public class Master : TypeObject<Master>
    {
        /// <summary>
        /// Gets the riskiness of the master.
        /// </summary>
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableFloat Riskiness { get; set; } = new GeneratableRangedFloat();

        /// <summary>
        /// Gets the investivity of the master.
        /// </summary>
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableFloat Investivity { get; set; } = new GeneratableRangedFloat();

        /// <summary>
        /// Gets the autonomy of the master.
        /// </summary>
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableFloat Autonomy { get; set; } = new GeneratableRangedFloat();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableProductionAgent ProductionAgent { get; set; }

        public override bool SupportsNaming => false;
    }
}