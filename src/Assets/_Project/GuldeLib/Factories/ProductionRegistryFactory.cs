using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ProductionRegistryFactory : Factory<ProductionRegistry, ProductionRegistryComponent>
    {
        public ProductionRegistryFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override ProductionRegistryComponent Create(ProductionRegistry productionRegistry)
        {
            foreach (var recipe in productionRegistry.Recipes)
            {
                Component.Register(recipe);
            }

            return Component;
        }
    }
}