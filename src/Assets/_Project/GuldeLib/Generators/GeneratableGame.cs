using System;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    [Serializable]
    public class GeneratableGame : GeneratableTypeObject<Game>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Game>();

            this.Log($"Game data generating.");

            if (Value.Naming?.IsGenerated ?? false) Value.Naming.Generate();
            if (Value.City.IsGenerated)
            {
                Value.City.Generate(Value);
            }

            this.Log($"Game data generated.");
        }
    }
}