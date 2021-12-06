using System;
using System.Linq;
using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Generators
{
    public class GeneratableInnName : GeneratableLocationName
    {
        public override void Generate()
        {
            if (!NameTable) return;

            var random = new Random();
            var randomIndex = random.Next(NameTable.InnNames.Count);

            Value = NameTable.InnNames.ElementAt(randomIndex);
        }
    }
}