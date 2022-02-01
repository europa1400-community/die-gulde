using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableFavor : GeneratableTypeObject<Favor>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Favor>();
        }
    }
}