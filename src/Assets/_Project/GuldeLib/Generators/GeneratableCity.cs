using GuldeLib.Cities;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    [Generatable]
    public class GeneratableCity : GeneratableTypeObject<City>
    {
        protected override bool SupportsDefaultGeneration => false;

        public override void Generate()
        {

        }

        public void Generate(Game game)
        {
            Value ??= ScriptableObject.CreateInstance<City>();

            this.Log($"City data generating.");

            if (Value.Naming?.IsGenerated ?? false) Value.Naming.Generate();
            if (Value.Time.IsGenerated) Value.Time.Generate();
            if (Value.Map.IsGenerated) Value.Map.Generate();

            foreach (var player in game.Players)
            {

            }

            this.Log($"City data generated.");
        }
    }
}