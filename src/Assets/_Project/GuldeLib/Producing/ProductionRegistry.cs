using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Producing
{
    public class ProductionRegistry : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public HashSet<Recipe> Recipes { get; set; } = new HashSet<Recipe>();
    }
}