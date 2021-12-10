using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableMap : GeneratableTypeObject<Map>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Map>();

            this.Log($"Map data generating.");

            if (Value.Naming.IsGenerated) Value.Naming.Generate();
            if (Value.EntityRegistry.IsGenerated) Value.EntityRegistry.Generate();

            if (Value.MapLayout.IsGenerated) Value.MapLayout.Generate(Value);

            if (Value.Market.IsGenerated) Value.Market.Generate();

            foreach (var workerHome in Value.WorkerHomes)
            {
                if (!workerHome?.IsGenerated ?? true) continue;
                workerHome.Generate();
            }

            foreach (var company in Value.Companies)
            {
                if (!company?.IsGenerated ?? true) continue;
                company.Generate();
            }

            if (Value.Nav.IsGenerated) Value.Nav.Generate();

            this.Log($"Map data generated.");
        }

        protected override bool SupportsDefaultGeneration => true;
    }
}