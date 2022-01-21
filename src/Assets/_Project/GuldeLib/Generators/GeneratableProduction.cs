using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableProduction : GeneratableTypeObject<Production>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Production>();

            this.Log($"Production data generating.");

            if (Value.Exchange?.IsGenerated ?? false) Value.Exchange.Generate();
            if (Value.Assignment?.IsGenerated ?? false) Value.Assignment.Generate();
            if (Value.ProductionRegistry?.IsGenerated ?? false) Value.ProductionRegistry.Generate();

            this.Log($"Production data generated.");
        }
    }
}