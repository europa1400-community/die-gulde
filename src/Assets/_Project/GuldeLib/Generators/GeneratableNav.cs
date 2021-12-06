using System;
using Codice.Client.BaseCommands;
using GuldeLib.Pathfinding;

namespace GuldeLib.Generators
{
    public class GeneratableNav : GeneratableTypeObject<Nav>
    {
        protected override bool SupportsDefaultGeneration => false;

        public override void Generate()
        {

        }

        public void Generate(Map map)
        {
            throw new NotImplementedException("NavMap generation has not been implemented");
        }
    }
}