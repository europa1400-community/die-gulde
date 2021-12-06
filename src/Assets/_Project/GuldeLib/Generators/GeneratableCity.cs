using GuldeLib.Cities;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    [Generatable]
    public class GeneratableCity : GeneratableTypeObject<City>
    {
        protected override bool SupportsDefaultGeneration => false;

        public override void Generate()
        {

        }

        public void Generate(Game game)
        {
            Value ??= ScriptableObject.CreateInstance<City>();

            this.Log($"City data generating.");

            if (Value.Naming?.IsGenerated ?? false) Value.Naming.Generate();
            if (Value.Map.IsGenerated) Value.Map.Generate();
            if (Value.Market.IsGenerated) Value.Market.Generate();
            if (Value.Time.IsGenerated) Value.Time.Generate();

            foreach (var player in game.Players)
            {

            }

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

            this.Log($"City data generated.");
        }
    }
}