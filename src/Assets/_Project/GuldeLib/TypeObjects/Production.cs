using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Producing/Production")]
    public class Production : TypeObject<Production>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableExchange Exchange { get; set; } = new GeneratableExchange();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableAssignment Assignment { get; set; } = new GeneratableAssignment();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableProductionRegistry ProductionRegistry { get; set; } = new GeneratableProductionRegistry();
        
        public override bool SupportsNaming => false;
    }
}