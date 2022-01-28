using System.Collections.Generic;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName="Producing/Recipe")]
    public class Recipe : SerializedScriptableObject
    {
        [Required]
        [Setting]
        [OdinSerialize]
        public string Name { get; set; }

        [Required]
        [Setting]
        [OdinSerialize]
        public Dictionary<Item, int> Resources { get; set; } = new Dictionary<Item, int>();

        [Required]
        [Setting]
        [OdinSerialize]
        public Item Product { get; set; }

        [Required]
        [Setting]
        [OdinSerialize]
        public bool IsExternal { get; set; }

        [MinValue(0f)]
        [Required]
        [Setting]
        [OdinSerialize]
        public float Time { get; set; }

        [FoldoutGroup("Info")]
        [ShowInInspector]
        public float MeanProfitPerHour
        {
            get
            {
                if (!Product) return 0;

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