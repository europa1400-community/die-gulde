using GuldeLib.Generators;
using GuldeLib.Producing;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Economy/Item")]
    public class Item : SerializedScriptableObject
    {
        [Required]
        [Setting]
        [OdinSerialize]
        public string Name { get; set; }

        [Required]
        [Setting]
        [OdinSerialize]
        public ItemType ItemType { get; set; }

        [MinValue("MinPrice")]
        [Required]
        [Setting]
        [OdinSerialize]
        public float MeanPrice { get; set; }

        [MinValue(0f)]
        [MaxValue("MeanPrice")]
        [Required]
        [Setting]
        [OdinSerialize]
        public float MinPrice { get; set; }

        [MinValue(0f)]
        [Required]
        [Setting]
        [OdinSerialize]
        public int MeanSupply { get; set; }

        [FoldoutGroup("Info")]
        [ShowInInspector]
        public float MaxPrice => 2 * MeanPrice - MinPrice;
    }
}
