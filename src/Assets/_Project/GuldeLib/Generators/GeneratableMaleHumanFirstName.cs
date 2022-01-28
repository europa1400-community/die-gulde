using System;
using System.Linq;

namespace GuldeLib.Generators
{
    public class GeneratableMaleHumanFirstName : GeneratableHumanName
    {
        public override void Generate()
        {
            if (!NameTable) return;

            var random = new Random();
            var randomIndex = random.Next(NameTable.MaleFirstNames.Count);

            Value = NameTable.MaleFirstNames.ElementAt(randomIndex);
        }
    }
}