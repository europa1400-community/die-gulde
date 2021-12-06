using GuldeLib.Companies.Employees;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableEmployee : GeneratableTypeObject<Employee>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Employee>();

            this.Log($"Employee data generating.");

            if (Value.Person.IsGenerated) Value.Person.Generate();
            if (Value.Pathfinder.IsGenerated) Value.Pathfinder.Generate();

            this.Log($"Employee data generated.");
        }
    }
}