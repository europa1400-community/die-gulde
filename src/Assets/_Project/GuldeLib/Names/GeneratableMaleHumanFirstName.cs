using System;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Names
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