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
        [BoxGroup("Settings")]
        public string Name { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        public Sprite Icon { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        public ItemType ItemType { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue("MinPrice")]
        public float MeanPrice { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue(0f)]
        [MaxValue("MeanPrice")]
        public float MinPrice { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue(0f)]
        public int MeanSupply { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public float MaxPrice => 2 * MeanPrice - MinPrice;
    }
}
