using GuldeLib.Inventories;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableInventory : GeneratableTypeObject<Inventory>
    {
        public override void Generate()
        {
            Value ??= ScriptableObject.CreateInstance<Inventory>();

            this.Log($"Inventory data generating.");

            this.Log($"Inventory data generated.");
        }
    }
}