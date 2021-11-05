using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Company
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