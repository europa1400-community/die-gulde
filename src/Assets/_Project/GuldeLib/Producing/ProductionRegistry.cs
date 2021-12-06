using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Producing
{
    [CreateAssetMenu(fileName="production_registry", menuName="Producing/ProductionRegistry")]
    public class ProductionRegistry : TypeObject<ProductionRegistry>
    {
        [Required]
        [OdinSerialize]
        public HashSet<Recipe> Recipes { get; set; } = new HashSet<Recipe>();
    }
}