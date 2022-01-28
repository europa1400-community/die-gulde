using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableBuilding : GeneratableTypeObject<Building>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Building>();
        }
    }
}