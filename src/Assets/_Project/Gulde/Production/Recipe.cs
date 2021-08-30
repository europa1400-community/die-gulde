using System.Collections.Generic;
using Gulde.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Production
{
    [CreateAssetMenu(menuName="Recipe")]
    public class Recipe : SerializedScriptableObject
    {
        [OdinSerialize]
        [Required]
        public Dictionary<Item, int> Resources { get; set; } = new Dictionary<Item, int>();

        [OdinSerialize]
        [Required]
        public Item Product { get; set; }

        [OdinSerialize]
        [MinValue(0f)]
        public float Time { get; set; }

        [OdinSerialize]
        public bool IsExternal { get; set; }

        [ShowInInspector]
        public float MeanProfitPerHour
        {
            get
            {
                if (!Product || Resources == null) return 0;

                var resourceCost = 0f;

                foreach (var pair in Resources)
                {
                    var item = pair.Key;
                    var amount = pair.Value;

                    resourceCost += item.MeanPrice * amount;
                }

                var productRevenue = Product.MeanPrice;

                return (productRevenue - resourceCost) / Time;
            }
        }
    }
}