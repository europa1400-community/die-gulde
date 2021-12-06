using GuldeLib.Producing;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableProduction : GeneratableTypeObject<Production>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Production>();

            this.Log($"Production data generating.");

            this.Log($"Production data generated.");
        }
    }
}