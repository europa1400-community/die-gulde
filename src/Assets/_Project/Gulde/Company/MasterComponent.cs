using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gulde.Company
{
    public class MasterComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Settings")]
        public float Riskiness { get; private set; }

        [OdinSerialize]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Settings")]
        public float Investivity { get; private set; }

        [OdinSerialize]
        [PropertyRange(0f, 1f)]
        [BoxGroup("Settings")]
        public float Autonomy { get; private set; }
    }
}