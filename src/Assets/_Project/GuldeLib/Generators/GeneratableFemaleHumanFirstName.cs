using System;
using System.Linq;
using GuldeLib.Names;

namespace GuldeLib.Generators
{
    public class GeneratableFemaleHumanFirstName : GeneratableHumanName
    {
        public override void Generate()
        {
            if (!NameTable) return;

            var random = new Random();
            var randomIndex = random.Next(NameTable.FemaleFirstNames.Count);

            Value = NameTable.FemaleFirstNames.ElementAt(randomIndex);
        }
    }
}