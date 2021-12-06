using System;
using GuldeLib.Companies;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    [Serializable]
    public class GeneratableCompany : GeneratableTypeObject<Company>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Company>();

            this.Log($"Company data generating.");

            if (Value.Location.IsGenerated) Value.Location.Generate();

            foreach (var cart in Value.Carts)
            {
                if (!cart?.IsGenerated ?? true) continue;
                cart.Generate();
            }

            foreach (var employee in Value.Employees)
            {
                if (!employee?.IsGenerated ?? true) continue;
                employee.Generate();
            }

            if (Value.Master?.IsGenerated ?? false) Value.Master.Generate();
            if (Value.Production?.IsGenerated ?? false) Value.Production.Generate();
            if (Value.Naming?.IsGenerated ?? false) Value.Naming.Generate();

            this.Log($"Company data generated.");
        }
    }
}