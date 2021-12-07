using System.Collections.Generic;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Economy/Wealth")]
    public class Wealth : TypeObject<Wealth>
    {
        [Required]
        [Setting]
        [OdinSerialize]
        public float Money { get; set; }

        [Optional]
        [Generatables]
        [OdinSerialize]
        public List<GeneratableCompany> Companies { get; set; }

        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableExchange Exchange { get; set; }

        public override bool SupportsNaming => false;
    }
}