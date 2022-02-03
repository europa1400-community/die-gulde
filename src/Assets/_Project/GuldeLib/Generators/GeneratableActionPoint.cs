using GuldeLib.Players;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableActionPoint : GeneratableTypeObject<ActionPoint>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<ActionPoint>();

            this.Log($"Action data generating");

            this.Log($"Action data generated");
        }
    }
}