using GuldeLib.Pathfinding;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratablePathfinder : GeneratableTypeObject<Pathfinder>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Pathfinder>();

            this.Log($"Pathfinder data generating.");

            if (Value.Entity.IsGenerated) Value.Entity.Generate();
            if (Value.Speed.IsGenerated) Value.Speed.Generate();

            this.Log($"Pathfinder data generated.");
        }
    }
}