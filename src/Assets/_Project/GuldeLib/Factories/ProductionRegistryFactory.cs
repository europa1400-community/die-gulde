using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ProductionRegistryFactory : Factory<ProductionRegistry>
    {
        public ProductionRegistryFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(ProductionRegistry productionRegistry)
        {
            var productionRegistryComponent = GameObject.AddComponent<ProductionRegistryComponent>();

            productionRegistryComponent.Recipes = productionRegistry.Recipes;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}