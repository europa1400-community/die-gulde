using Sirenix.OdinInspector;

namespace GuldeLib.Companies
{
    /// <summary>
    /// Provides information for masters.
    /// </summary>
    public class MasterComponent : SerializedMonoBehaviour
    {
        /// <summary>
        /// Gets the riskiness of the master.
        /// </summary>
        [ShowInInspector]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Settings")]
        public float Riskiness { get; private set; }

        /// <summary>
        /// Gets the investivity of the master.
        /// </summary>
        [ShowInInspector]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Settings")]
        public float Investivity { get; private set; }

        /// <summary>
        /// Gets the autonomy of the master.
        /// </summary>
        [ShowInInspector]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Settings")]
        public float Autonomy { get; private set; }
    }
}