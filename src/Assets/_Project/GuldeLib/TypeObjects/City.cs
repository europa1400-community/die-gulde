using System.Collections.Generic;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Cities/City")]
    public class City : TypeObject<City>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableMap Map { get; set; } = new GeneratableMap();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableTime Time { get; set; } = new GeneratableTime();

        public override bool SupportsNaming => true;
    }
}