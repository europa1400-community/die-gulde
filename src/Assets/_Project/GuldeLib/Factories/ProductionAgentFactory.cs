using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ProductionAgentFactory : Factory<ProductionAgent, ProductionAgentComponent>
    {
        public ProductionAgentFactory(ProductionAgent productionAgent, GameObject gameObject) : base(
            productionAgent, gameObject, null)
        {
        }

        public override ProductionAgentComponent Create()
        {
            Component.ResourceBuffer = TypeObject.ResourceBuffer;

            var productionRegistryComponent = GameObject.GetComponent<ProductionRegistryComponent>();

            productionRegistryComponent.RecipeFinished += Component.OnRecipeFinished;

            return Component;
        }
    }
}