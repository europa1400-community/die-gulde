using GuldeLib.Entities;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableEntity : GeneratableTypeObject<Entity>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Entity>();

            this.Log($"Entity data generating.");

            if (Value.Naming?.IsGenerated ?? false) Value.Naming.Generate();

            this.Log($"Entity data generated.");
        }
    }
}