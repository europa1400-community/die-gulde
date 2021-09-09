using GuldeLib.Production;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Economy
{
    [CreateAssetMenu(menuName = "Economy/Product")]
    public class Item : SerializedScriptableObject
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        public string Name { get; private set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        public ItemType ItemType { get; private set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue("MinPrice")]
        public float MeanPrice { get; private set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue(0f)]
        [MaxValue("MeanPrice")]
        public float MinPrice { get; private set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue(0f)]
        public int MeanSupply { get; private set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public float MaxPrice => 2 * MeanPrice - MinPrice;
    }
}
