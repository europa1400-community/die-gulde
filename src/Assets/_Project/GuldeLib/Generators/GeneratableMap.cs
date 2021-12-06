

using GuldeLib.Maps;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableMap : GeneratableTypeObject<Map>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Map>();

            this.Log($"Map data generating.");

            if (Value.Naming.IsGenerated) Value.Naming.Generate();
            if (Value.EntityRegistry.IsGenerated) Value.EntityRegistry.Generate();

            if (Value.MapLayout.IsGenerated) Value.MapLayout.Generate();

            if (Value.Nav.IsGenerated) Value.Nav.Generate();

            this.Log($"Map data generated.");
        }
    }
}