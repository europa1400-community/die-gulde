using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableAssignment : GeneratableTypeObject<Assignment>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Assignment>();

            this.Log($"Assignment data generating");

            this.Log($"Assignment data generated");
        }
    }
}