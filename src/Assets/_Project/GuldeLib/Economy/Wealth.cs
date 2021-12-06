using System.Collections.Generic;
using GuldeLib.Companies;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Economy
{
    [CreateAssetMenu(menuName = "Economy/Wealth")]
    public class Wealth : TypeObject<Wealth>
    {
        [Required]
        [OdinSerialize]
        public float Money { get; set; }

        [Optional]
        [OdinSerialize]
        public List<GeneratableCompany> Companies { get; set; }

        [Optional]
        [OdinSerialize]
        public GeneratableExchange Exchange { get; set; }
    }
}