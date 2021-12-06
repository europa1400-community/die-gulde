using GuldeLib.Players.Professions;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableProfession : GeneratableTypeObject<Profession>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Profession>();

            this.Log($"Profession data generating.");

            if (Value.Level?.IsGenerated ?? false) Value.Level.Generate();

            this.Log($"Profession data generated.");
        }
    }
}