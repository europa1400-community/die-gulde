using GuldeLib.Names;
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

            var locationNaming = (LocationNaming) Value;

            if (locationNaming.LocationName?.IsGenerated ?? false) locationNaming.LocationName.Generate();

            this.Log($"LocationNaming data generated.");
        }
    }
}