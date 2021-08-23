using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gulde.Company;
using Gulde.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Gulde.Production
{
    public class ProductionRegistryComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Info")]
        public HashSet<Recipe> Recipes { get; private set; } = new HashSet<Recipe>();

        [OdinSerialize]
        [BoxGroup("Info")]
        Dictionary<Recipe, Coroutine> ProductionRoutines { get; set; } = new Dictionary<Recipe, Coroutine>();

        [OdinSerialize]
        [BoxGroup("Info")]
        Dictionary<Recipe, int> ProductionPercentages { get; set; } = new Dictionary<Recipe, int>();

        [OdinSerialize]
        [FoldoutGroup("Debug")]
        AssignmentComponent Assignment { get; set; }

        [OdinSerialize]
        [FoldoutGroup("Debug")]
        CompanyComponent Company { get; set; }

        public event EventHandler<ProductionEventArgs> RecipeFinished;

        public Recipe GetRecipe(Item item) =>
            Recipes.ToList().Find(e => e.Product == item);

        public bool IsRegistered(Recipe recipe) =>
            Recipes.Contains(recipe) && ProductionRoutines.ContainsKey(recipe) && ProductionPercentages.ContainsKey(recipe);

        public bool IsProducing(Recipe recipe) =>
            ProductionRoutines.ContainsKey(recipe) && ProductionRoutines[recipe] != null;

        public bool HasProgress(Recipe recipe) =>
            ProductionPercentages.ContainsKey(recipe) && ProductionPercentages[recipe] != 0;

        public List<Recipe> ActiveRecipes =>
            ProductionRoutines.Where(pair => IsProducing(pair.Key)).Select(pair => pair.Key).ToList();

        public HashSet<Recipe> HaltedRecipes =>
            Assignment.GetAssignedRecipes.Where(e => !IsProducing(e)).ToHashSet();

        void Awake()
        {
            Assignment = GetComponent<AssignmentComponent>();
            Company = GetComponent<CompanyComponent>();

            foreach (var recipe in Recipes)
            {
                Register(recipe);
            }
        }

        public void Register(Recipe recipe)
        {
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
            if (!IsRegistered(recipe)) return;

            var percentage = ProductionPercentages[recipe];
            var coroutine = StartCoroutine(ProductionRoutine(recipe, percentage));
            ProductionRoutines[recipe] = coroutine;
        }

        public void StopProductionRoutine(Recipe recipe)
        {
            if (!IsRegistered(recipe)) return;

            StopCoroutine(ProductionRoutines[recipe]);
            ProductionRoutines[recipe] = null;
        }

        public void ResetProgress(Recipe recipe)
        {
            if (!IsRegistered(recipe)) return;

            ProductionPercentages[recipe] = 0;
        }

        public void StopProductionRoutines()
        {
            foreach (var recipe in ActiveRecipes)
            {
                StopProductionRoutine(recipe);
            }
        }

        IEnumerator ProductionRoutine(Recipe recipe, int startPercentage = 0)
        {
            Register(recipe);
            ProductionPercentages[recipe] = startPercentage;

            while (ProductionPercentages[recipe] < 100)
            {
                var assignmentCount = Assignment.AssignmentCount(recipe);
                var step = recipe.Time / 100 / Mathf.Max(assignmentCount, 1);

                yield return new WaitForSeconds(step);

                ProductionPercentages[recipe] += 1;
                Debug.Log($"{recipe.name} is {ProductionPercentages[recipe]}% done.");
            }

            ResetProgress(recipe);

            var employees = Assignment.GetAssignedEmployees(recipe);
            RecipeFinished?.Invoke(this, new ProductionEventArgs(recipe, employees));
        }
    }
}