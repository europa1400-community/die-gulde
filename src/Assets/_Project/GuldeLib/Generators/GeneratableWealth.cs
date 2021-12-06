using GuldeLib.Economy;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableWealth : GeneratableTypeObject<Wealth>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Wealth>();

            this.Log($"Wealth data generating.");

            if (Value.Exchange?.IsGenerated ?? false) Value.Exchange.Generate();

            foreach (var company in Value.Companies)
            {
                if (!company?.IsGenerated ?? true) continue;
                company.Generate();
            }

            this.Log($"Wealth data generated.");
        }
    }
}