using System.Collections;
using System.Collections.Generic;
using Gulde.Inventory;
using Gulde.Production;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Economy
{
    [CreateAssetMenu(menuName = "Economy/Product")]
    public class Item : SerializedScriptableObject
    {
        [OdinSerialize]
        public string Name { get; set; }

        [OdinSerialize]
        public Sprite Icon { get; set; }

        [OdinSerialize]
        public ItemType ItemType { get; set; }

        [OdinSerialize]
        [MinValue("MinPrice")]
        public float MeanPrice { get; set; }

        [OdinSerialize]
        [MinValue(0f)]
        [MaxValue("MeanPrice")]
        public float MinPrice { get; set; }

        [ShowInInspector]
        public float MaxPrice => 2 * MeanPrice - MinPrice;

        [OdinSerialize]
        [MinValue(0f)]
        public int MeanSupply { get; set; }
    }
}
