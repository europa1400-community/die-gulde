using GuldeLib.TypeObjects;
using GuldeLib.WorkerHomes;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableWorkerHome : GeneratableTypeObject<WorkerHome>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<WorkerHome>();

            this.Log($"WorkerHome data generating.");

            if (Value.Location.IsGenerated) Value.Location.Generate();

            this.Log($"WorkerHome data generated.");
        }
    }
}