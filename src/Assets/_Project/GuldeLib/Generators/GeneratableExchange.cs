using GuldeLib.Economy;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableExchange : GeneratableTypeObject<Exchange>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Exchange>();

            this.Log($"Exchange data generating.");

            if (Value.Naming?.IsGenerated ?? false) Value.Naming.Generate();
            if (Value.Inventory.IsGenerated) Value.Inventory.Generate();
            if (Value.ProductInventory?.IsGenerated ?? false) Value.ProductInventory.Generate();

            this.Log($"Exchange data generated.");
        }
    }
}