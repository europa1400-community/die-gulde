using System.Security.Cryptography;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Generators
{
    public class GeneratableRangedFloat : GeneratableFloat
    {
        [OdinSerialize]
        [HorizontalGroup("Generation/Range")]
        public float MinValue { get; set; }

        [OdinSerialize]
        [HorizontalGroup("Generation/Range")]
        public float MaxValue { get; set; }

        public override void Generate()
        {
            Value = Random.Range(MinValue, MaxValue);
        }

        public static implicit operator GeneratableRangedFloat(float value) => new GeneratableRangedFloat {Value = value};
    }
}