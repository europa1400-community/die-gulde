using GuldeLib.Names;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableNaming : GeneratableTypeObject<Naming>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Naming>();

            this.Log($"Naming data generating.");

            if (Value.Name?.IsGenerated ?? false) Value.Name.Generate();
            if (Value.FriendlyName?.IsGenerated ?? false) Value.FriendlyName.Generate();

            this.Log($"Naming data generated.");
        }
    }
}