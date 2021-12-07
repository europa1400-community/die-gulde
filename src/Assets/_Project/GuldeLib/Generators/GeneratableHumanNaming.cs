using GuldeLib.Names;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableHumanNaming : GeneratableNaming
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<HumanNaming>();

            this.Log($"HumanNaming data generating.");

            if (!(Value is HumanNaming humanNaming)) return;

            humanNaming.FirstName.Generate();
            humanNaming.LastName.Generate();

            this.Log($"HumanNaming data generated.");
        }
    }
}