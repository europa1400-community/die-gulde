using System;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableNav : GeneratableTypeObject<Nav>
    {
        protected override bool SupportsDefaultGeneration => false;

        public override void Generate()
        {
            Value = ScriptableObject.CreateInstance<Nav>();
        }

        public void Generate(Map map)
        {
            throw new NotImplementedException("NavMap generation has not been implemented");
        }
    }
}