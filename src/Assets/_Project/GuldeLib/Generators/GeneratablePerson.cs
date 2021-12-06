using GuldeLib.Persons;
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

            if (Value.Naming.IsGenerated) Value.Naming.Generate();

            this.Log($"Person data generated.");
        }
    }
}