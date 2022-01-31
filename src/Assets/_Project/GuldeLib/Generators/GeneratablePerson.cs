using GuldeLib.Social;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratablePerson : GeneratableTypeObject<Person>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Person>();

            this.Log($"Person data generating.");

            if (Value.Naming?.IsGenerated ?? false) Value.Naming.Generate();

            this.Log($"Person data generated.");
        }
    }
}