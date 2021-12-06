using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableDeviationFloat : GeneratableFloat
    {
        [OdinSerialize]
        [Generatable]
        float Median { get; set; }

        [OdinSerialize]
        [Generatable]
        [SuffixLabel("%")]
        float MaxDeviation { get; set; }

        public override void Generate()
        {
            var deviation = Median * Random.Range(-MaxDeviation, MaxDeviation);
            Value = Median + deviation;
        }
    }
}