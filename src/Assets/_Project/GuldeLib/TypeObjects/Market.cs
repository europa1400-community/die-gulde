using System.Collections.Generic;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Economy/Market")]
    public class Market : TypeObject<Market>
    {
        [Required]
        [Generatables]
        [OdinSerialize]
        public List<GeneratableExchange> Exchanges { get; set; } = new List<GeneratableExchange>();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableLocation Location { get; set; } = new GeneratableLocation();

        public override bool SupportsNaming => false;
    }
}