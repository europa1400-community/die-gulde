using System.Collections.Generic;
using GuldeLib.Generators;
using GuldeLib.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Economy
{
    [CreateAssetMenu(menuName = "Economy/Market")]
    public class Market : TypeObject<Market>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public List<GeneratableExchange> Exchanges { get; set; } = new List<GeneratableExchange>();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableLocation Location { get; set; } = new GeneratableLocation();
    }
}