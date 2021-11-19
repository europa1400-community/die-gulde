using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableFloat : Generatable<float>
    {
        public override void Generate()
        {
            Value = Random.Range(float.MinValue, float.MaxValue);
        }

        public static implicit operator GeneratableFloat(float value) => new GeneratableFloat { Value = value };
    }
}