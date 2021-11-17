using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Economy;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace GuldeLib.Producing
{
    public class ProductionRegistryComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<Recipe> Recipes { get; set; } = new HashSet<Recipe>();

        [ShowInInspector]
        [BoxGroup("Info")]
        Dictionary<Recipe, Coroutine> ProductionRoutines { get; } = new Dictionary<Recipe, Coroutine>();

        [ShowInInspector]
        [BoxGroup("Info")]
        Dictionary<Recipe, float> ProductionPercentages { get; } = new Dictionary<Recipe, float>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        AssignmentComponent Assignment { get; set; }

        public event EventHandler<ProductionEventArgs> RecipeFinished;

        public WaitForRecipeFinished WaitForRecipeFinished(Recipe recipe) => new WaitForRecipeFinished(this, recipe);

        public Recipe GetRecipe(Item item) =>
            Recipes.ToList().Find(e => e.Product == item);

        public bool IsRegistered(Recipe recipe) =>
            Recipes.Contains(recipe) && ProductionRoutines.ContainsKey(recipe) && ProductionPercentages.ContainsKey(recipe);

        public bool IsProducing(Recipe recipe) =>
            recipe && ProductionRoutines.ContainsKey(recipe) && ProductionRoutines[recipe] != null;

        public bool HasProgress(Recipe recipe) =>
            ProductionPercentages.ContainsKey(recipe) && ProductionPercentages[recipe] != 0;

        public List<Recipe> ActiveRecipes =>
            ProductionRoutines.Where(pair => IsProducing(pair.Key)).Select(pair => pair.Key).ToList();

        public HashSet<Recipe> HaltedRecipes =>
            Assignment.AssignedRecipes.Where(e => !IsProducing(e)).ToHashSet();

        void Awake()
        {
            this.Log("Production registry initializing");

            Assignment = GetComponent<AssignmentComponent>();
        }

        public void Register(Recipe recipe)
        {
            this.Log($"Production registry registering {recipe}");

            if (!Recipes.Contains(recipe)) Recipes.Add(recipe);
            if (!ProductionRoutines.ContainsKey(recipe)) ProductionRoutines.Add(recipe, null);
            if (!ProductionPercentages.ContainsKey(recipe)) ProductionPercentages.Add(recipe, 0);
        }

        public void Register(HashSet<Recipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                Register(recipe);
            }
        }

        public void StartProductionRoutine(Recipe recipe)
        {
            this.Log($"Production registry starting production for {recipe}");

            if (!IsRegistered(recipe))
            {
                this.Log($"Production can't start for {recipe}: Recipe was not registerd", LogType.Warning);
                return;
            }

            var percentage = ProductionPercentages[recipe];
            var coroutine = StartCoroutine(ProductionRoutine(recipe, percentage));
            ProductionRoutines[recipe] = coroutine;
        }

        public void StopProductionRoutine(Recipe recipe)
        {
            this.Log($"Production registry stopping production for {recipe}");

            if (!IsRegistered(recipe))
            {
                this.Log($"Production can't stop for {recipe}: Recipe was not registered", LogType.Warning);
                return;
            }

            StopCoroutine(ProductionRoutines[recipe]);
            ProductionRoutines[recipe] = null;
        }

        public void ResetProgress(Recipe recipe)
        {
            this.Log($"Production registry resetting progress for {recipe}");

            if (!IsRegistered(recipe))
            {
                this.Log($"Production can't reset for {recipe}: Recipe was not registered", LogType.Warning);
                return;
            }

            ProductionPercentages[recipe] = 0;
        }

        public void StopProductionRoutines()
        {
            this.Log($"Production registry stopping production for all recipes");

            foreach (var recipe in ActiveRecipes)
            {
                StopProductionRoutine(recipe);
            }
        }

        IEnumerator ProductionRoutine(Recipe recipe, float startPercentage = 0)
        {
            this.Log($"Production registry starting production for {recipe} with starting percentage of {startPercentage}");

            Register(recipe);

            var percentage = startPercentage == 0 ? 1 : startPercentage;
            ProductionPercentages[recipe] = percentage;

            while (ProductionPercentages[recipe] < 100)
            {
                yield return Locator.Time.WaitForMinuteTicked;

                var assignmentCount = Assignment.AssignmentCount(recipe);
                ProductionPercentages[recipe] +=  100f * assignmentCount / recipe.Time;
                this.Log($"Production registry progress for {recipe} now at {ProductionPercentages[recipe]}");
            }

            ResetProgress(recipe);

            var employees = Assignment.GetAssignedEmployees(recipe);
            RecipeFinished?.Invoke(this, new ProductionEventArgs(recipe, employees));
        }
    }

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