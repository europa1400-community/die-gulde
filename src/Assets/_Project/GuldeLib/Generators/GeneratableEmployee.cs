using GuldeLib.Companies.Employees;
using GuldeLib.TypeObjects;
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
            if (Value.Travel.IsGenerated) Value.Travel.Generate();

            this.Log($"Employee data generated.");
        }
    }
}