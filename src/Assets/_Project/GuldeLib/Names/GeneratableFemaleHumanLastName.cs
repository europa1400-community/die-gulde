using System;
using System.Linq;

namespace GuldeLib.Names
{
    public class GeneratableFemaleHumanLastName : GeneratableHumanName
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