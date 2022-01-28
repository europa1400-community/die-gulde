using System;
using System.Linq;
using GuldeLib.Names;

namespace GuldeLib.Generators
{
    public class GeneratableHumanLastName : GeneratableHumanName
    {
        public override void Generate()
        {
            if (!NameTable) return;

            var random = new Random();
            var randomIndex = random.Next(NameTable.LastNames.Count);

            Value = NameTable.LastNames.ElementAt(randomIndex);
        }
    }
}