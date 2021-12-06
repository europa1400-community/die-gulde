using GuldeLib.Entities;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableTravel : GeneratableTypeObject<Travel>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Travel>();

            this.Log($"Travel data generating.");

            if (Value.Pathfinder.IsGenerated) Value.Pathfinder.Generate();

            this.Log($"Travel data generated.");
        }
    }
}