using System.Collections;
using System.Collections.Generic;
using Gulde.Inventory;
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
        public float MeanPrice { get; set; }

        [OdinSerialize]
        public int MeanSupply { get; set; }

        [OdinSerialize]
        public float SupplyWeight { get; set; }
    }
}
