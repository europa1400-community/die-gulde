using System;
using System.Linq;
using GuldeLib.Generators;

namespace GuldeLib.Names
{
    public class GeneratableName : Generatable<string>
    {
        public override void Generate()
        {
            Value = new Random().Next().ToString();
        }

        public static implicit operator GeneratableName(string value) =>
            new GeneratableName { Value = value, IsGenerated = true };
    }
}