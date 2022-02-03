using GuldeLib.Players;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableCitizen : GeneratableTypeObject<Citizen>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Citizen>();

            this.Log($"Player data generating.");

            if (Value.ActionPoint.IsGenerated) Value.ActionPoint.Generate();
            if (Value.Wealth.IsGenerated) Value.Wealth.Generate();
            if (Value.Favor.IsGenerated) Value.Favor.Generate();
            if (Value.Talent.IsGenerated) Value.Talent.Generate();

            this.Log($"Player data generated.");
        }
    }
}