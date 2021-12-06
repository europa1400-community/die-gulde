using GuldeLib.Generators;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Companies
{
    [CreateAssetMenu(menuName = "Companies/Master")]
    public class Master : TypeObject<Master>
    {
        /// <summary>
        /// Gets the riskiness of the master.
        /// </summary>
        [Required]
        [Generatable]
        [ShowInInspector]
        [BoxGroup("Settings")]
        public GeneratableFloat Riskiness { get; set; } = new GeneratableRangedFloat();

        /// <summary>
        /// Gets the investivity of the master.
        /// </summary>
        [Required]
        [Generatable]
        [ShowInInspector]
        [BoxGroup("Settings")]
        public GeneratableFloat Investivity { get; set; } = new GeneratableRangedFloat();

        /// <summary>
        /// Gets the autonomy of the master.
        /// </summary>
        [Required]
        [Generatable]
        [ShowInInspector]
        [BoxGroup("Settings")]
        public GeneratableFloat Autonomy { get; set; } = new GeneratableRangedFloat();
    }
}