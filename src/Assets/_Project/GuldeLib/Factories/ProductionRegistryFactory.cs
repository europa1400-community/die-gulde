using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ProductionRegistryFactory : Factory<ProductionRegistry, ProductionRegistryComponent>
    {
        public ProductionRegistryFactory(ProductionRegistry productionRegistry, GameObject gameObject = null, GameObject parentObject = null) : base(productionRegistry, gameObject, parentObject)
        {
        }

        public override ProductionRegistryComponent Create()
        {
            foreach (var recipe in TypeObject.Recipes)
            {
                Component.Register(recipe);
            }

            return Component;
        }
    }
}