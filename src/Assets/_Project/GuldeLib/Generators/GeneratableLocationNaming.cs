using GuldeLib.Names;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableLocationNaming : GeneratableNaming
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<LocationNaming>();

            this.Log($"LocationNaming data generating.");

            if (!(Value is LocationNaming locationNaming)) return;

            if (locationNaming.LocationName?.IsGenerated ?? false) locationNaming.LocationName.Generate();

            this.Log($"LocationNaming data generated.");
        }
    }
}