using System;

namespace GuldeLib.Generators
{
    public class GeneratableName : Generatable<string>
    {
        public override void Generate()
        {
            Value = new Random().Next().ToString();
        }
    }
}