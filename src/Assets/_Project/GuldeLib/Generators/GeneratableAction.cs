using GuldeLib.Players;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableAction : GeneratableTypeObject<Action>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Action>();

            this.Log($"Action data generating");

            this.Log($"Action data generated");
        }
    }
}