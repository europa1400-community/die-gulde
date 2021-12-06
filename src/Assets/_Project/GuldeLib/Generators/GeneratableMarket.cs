using GuldeLib.Economy;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableMarket : GeneratableTypeObject<Market>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Market>();

            this.Log($"Market data generating.");

            if (Value.Location.IsGenerated)
            {
                this.Log("ja", LogType.Exception);
                Value.Location.Generate();
            }

            foreach (var exchange in Value.Exchanges)
            {
                if (!exchange?.IsGenerated ?? true) continue;
                exchange.Generate();
            }

            this.Log($"Market data generated.");
        }
    }
}