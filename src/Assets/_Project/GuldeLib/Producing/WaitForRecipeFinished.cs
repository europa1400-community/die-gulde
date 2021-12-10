using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Producing
{
    public class WaitForRecipeFinished : CustomYieldInstruction
    {
        Recipe Recipe { get; }
        ProductionRegistryComponent ProductionRegistry { get; }
        bool IsRecipeFinished { get; set; }
        public override bool keepWaiting => ProductionRegistry.IsProducing(Recipe) && !IsRecipeFinished;

        public WaitForRecipeFinished(ProductionRegistryComponent productionRegistry, Recipe recipe)
        {
            Recipe = recipe;
            ProductionRegistry = productionRegistry;
            productionRegistry.RecipeFinished += OnRecipeFinished;
        }

        void OnRecipeFinished(object sender, ProductionEventArgs e)
        {
            if (e.Recipe == Recipe) IsRecipeFinished = true;
        }
    }
}