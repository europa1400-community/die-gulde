using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableRangedInt : GeneratableInt
    {
        [OdinSerialize]
        [HorizontalGroup("Generation/Range")]
        public int MinValue { get; set; }

        [OdinSerialize]
        [HorizontalGroup("Generation/Range")]
        public int MaxValue { get; set; }

        public override void Generate()
        {
            Value = Random.Range(MinValue, MaxValue);
        }

        public static implicit operator GeneratableRangedInt(int value) => new GeneratableRangedInt {Value = value};
    }
}