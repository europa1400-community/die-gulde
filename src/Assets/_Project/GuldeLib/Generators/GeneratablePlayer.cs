using GuldeLib.Players;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratablePlayer : GeneratableTypeObject<Player>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Player>();

            this.Log($"Player data generating.");

            if (Value.Action.IsGenerated) Value.Action.Generate();
            if (Value.Wealth.IsGenerated) Value.Wealth.Generate();

            this.Log($"Player data generated.");
        }
    }
}