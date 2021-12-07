using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableDeviationFloat : GeneratableFloat
    {
        [BoxGroup("Generation")]
        [OdinSerialize]
        float Median { get; set; }

        [BoxGroup("Generation")]
        [SuffixLabel("%")]
        [OdinSerialize]
        float MaxDeviation { get; set; }

        public override void Generate()
        {
            var deviation = Median * Random.Range(-MaxDeviation, MaxDeviation);
            Value = Median + deviation;
        }
    }
}