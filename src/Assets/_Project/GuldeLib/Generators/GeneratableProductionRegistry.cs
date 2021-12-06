using GuldeLib.Producing;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableProductionRegistry : GeneratableTypeObject<ProductionRegistry>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<ProductionRegistry>();

            this.Log($"ProductionRegistry data generating.");

            this.Log($"ProductionRegistry data generated.");
        }
    }
}