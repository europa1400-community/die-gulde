using MonoLogger.Runtime;
using UnityEngine;
using Time = GuldeLib.Timing.Time;

namespace GuldeLib.Generators
{
    public class GeneratableTime : GeneratableTypeObject<Time>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Time>();

            this.Log($"Time data generating.");

            if (Value.Naming?.IsGenerated ?? false) Value.Naming.Generate();
            if (Value.Hour.IsGenerated) Value.Hour.Generate();
            if (Value.Minute.IsGenerated) Value.Minute.Generate();
            if (Value.Year.IsGenerated) Value.Year.Generate();
            if (Value.TimeSpeed.IsGenerated) Value.TimeSpeed.Generate();
            if (Value.NormalTimeSpeed.IsGenerated) Value.NormalTimeSpeed.Generate();

            this.Log($"Time data generated.");
        }
    }
}