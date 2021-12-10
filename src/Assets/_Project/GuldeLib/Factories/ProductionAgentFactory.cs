using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ProductionAgentFactory : Factory<ProductionAgent, ProductionAgentComponent>
    {
        public ProductionAgentFactory(GameObject gameObject) : base(gameObject, null)
        {
        }

        public override ProductionAgentComponent Create(ProductionAgent productionAgent)
        {
            Component.ResourceBuffer = productionAgent.ResourceBuffer;

            var productionRegistryComponent = GameObject.GetComponent<ProductionRegistryComponent>();

            productionRegistryComponent.RecipeFinished += Component.OnRecipeFinished;

            return Component;
        }
    }
}