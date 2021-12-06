using GuldeLib.Entities;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableEntityRegistry : GeneratableTypeObject<EntityRegistry>
    {
        protected override bool IsTemporary => false;

        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<EntityRegistry>();

            this.Log($"EntityRegistry data generating.");

            this.Log($"EntityRegistry data generated.");
        }
    }
}