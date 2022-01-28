using GuldeLib.Companies.Carts;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableCart : GeneratableTypeObject<Cart>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Cart>();

            this.Log($"Cart data generating.");

            if (Value.Travel.IsGenerated) Value.Travel.Generate();

            this.Log($"Cart data generated.");
        }
    }
}