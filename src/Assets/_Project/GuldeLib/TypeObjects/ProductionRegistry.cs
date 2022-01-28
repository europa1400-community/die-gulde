using System.Collections.Generic;
using GuldeLib.Generators;
using GuldeLib.Producing;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(fileName="production_registry", menuName="Producing/ProductionRegistry")]
    public class ProductionRegistry : TypeObject<ProductionRegistry>
    {
        [Required]
        [Setting]
        [OdinSerialize]
        public HashSet<Recipe> Recipes { get; set; } = new HashSet<Recipe>();

        public override bool SupportsNaming => false;
    }
}