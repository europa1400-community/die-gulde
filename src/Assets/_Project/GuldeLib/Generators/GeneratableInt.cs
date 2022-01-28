using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableInt : Generatable<int>
    {
        public override void Generate()
        {
            Value = Random.Range(int.MinValue, int.MaxValue);
        }

        public static implicit operator GeneratableInt(int value) => new GeneratableInt {Value = value};
    }
}