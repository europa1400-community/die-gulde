using GuldeLib.Generators;
using UnityEngine;

namespace GuldeLib.Names
{
    public class GeneratableNaming : GeneratableTypeObject<Naming>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Naming>();
            Value.Name.Generate();
            Value.FriendlyName.Generate();
        }
    }
}