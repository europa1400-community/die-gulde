using UnityEngine;

namespace GuldeLib.Names
{
    public class GeneratableHumanNaming : GeneratableNaming
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<HumanNaming>();

            if (!(Value is HumanNaming humanNaming)) return;

            humanNaming.FirstName.Generate();
            humanNaming.LastName.Generate();
        }
    }
}