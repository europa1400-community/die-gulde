using Sirenix.OdinInspector;

namespace GuldeLib.Companies
{
    public class Master : SerializedScriptableObject
    {
        /// <summary>
        /// Gets the riskiness of the master.
        /// </summary>
        [Required]
        [ShowInInspector]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Settings")]
        public float Riskiness { get; set; }

        /// <summary>
        /// Gets the investivity of the master.
        /// </summary>
        [Required]
        [ShowInInspector]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Settings")]
        public float Investivity { get; set; }

        /// <summary>
        /// Gets the autonomy of the master.
        /// </summary>
        [Required]
        [ShowInInspector]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Settings")]
        public float Autonomy { get; set; }
    }
}