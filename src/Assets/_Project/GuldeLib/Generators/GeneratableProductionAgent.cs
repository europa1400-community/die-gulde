using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableProductionAgent : GeneratableTypeObject<ProductionAgent>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<ProductionAgent>();
        }
    }
}