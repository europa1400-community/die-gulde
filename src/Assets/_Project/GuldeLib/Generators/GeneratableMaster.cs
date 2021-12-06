using GuldeLib.Companies;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableMaster : GeneratableTypeObject<Master>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Master>();

            this.Log($"Master data generating.");

            if (Value.Autonomy.IsGenerated) Value.Autonomy.Generate();
            if (Value.Investivity.IsGenerated) Value.Investivity.Generate();
            if (Value.Riskiness.IsGenerated) Value.Riskiness.Generate();

            this.Log($"Master data generated.");
        }
    }
}