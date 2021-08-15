using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gulde.Inventory;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gulde.Production
{
    [HideMonoScript]
    public class ProductionComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        InventoryComponent ResourceInventory { get; set; }

        [OdinSerialize]
        InventoryComponent ProductInventory { get; set; }

        [OdinSerialize]
        public HashSet<Recipe> Recipes { get; set; } = new HashSet<Recipe>();

        [OdinSerialize]
        Dictionary<EmployeeComponent, Recipe> Assignments { get; set; } = new Dictionary<EmployeeComponent, Recipe>();

        [OdinSerialize]
        Dictionary<Recipe, Coroutine> ProductionRoutines { get; set; } = new Dictionary<Recipe, Coroutine>();

        public event EventHandler<RecipeEventArgs> RecipeFinished;
        public event EventHandler FeierabendReached;

        public bool IsEmployeeAssigned(EmployeeComponent employee) => Assignments.ContainsKey(employee);

        bool IsProducing(Recipe recipe) => ProductionRoutines.ContainsKey(recipe) && ProductionRoutines[recipe] != null;

        InventoryComponent TargetInventory(Recipe recipe) =>
            recipe.InventoryType == InventoryType.Product ? ProductInventory : ResourceInventory;

        public bool CanProduce(Recipe recipe)
        {
            foreach (var pair in recipe.Resources)
            {
                var resource = pair.Key;
                var amount = pair.Value;

                if (ResourceInventory.GetSupply(resource) < amount) return false;
            }

            var targetInventory = TargetInventory(recipe);
            return targetInventory.HasProduct(recipe.Product) || !targetInventory.IsFull;
        }

        public int AssignedEmployees(Recipe recipe) => Assignments.Count(e => e.Value == recipe);

        void Awake()
        {
            RecipeFinished -= OnRecipeFinished;
            RecipeFinished += OnRecipeFinished;
        }

        void Update()
        {
            if (Keyboard.current.spaceKey.isPressed)
            {
                FeierabendReached?.Invoke(this, EventArgs.Empty);
            }
        }

        public void AssignEmployee(EmployeeComponent employee, Recipe recipe)
        {
            if (!IsEmployeeAssigned(employee)) Assignments.Add(employee, recipe);
            else UnassignEmployee(employee);

            Assignments[employee] = recipe;

            if (!IsProducing(recipe)) StartProduction(recipe);
        }

        public void UnassignEmployee(EmployeeComponent employee)
        {
            if (!IsEmployeeAssigned(employee)) Assignments.Add(employee, null);

            var recipe = Assignments[employee];

            Assignments[employee] = null;

            if (recipe && AssignedEmployees(recipe) == 0) StopProduction(recipe);
        }

        void StartProduction(Recipe recipe)
        {
            if (!ProductionRoutines.ContainsKey(recipe)) ProductionRoutines.Add(recipe, null);
            if (ProductionRoutines[recipe] != null) return;
            if (!CanProduce(recipe)) return;

            var targetInventory = TargetInventory(recipe);
            targetInventory.Register(recipe.Product);

            foreach (var pair in recipe.Resources)
            {
                var resource = pair.Key;
                var amount = pair.Value;

                for (var i = 0; i < amount; i++)
                {
                    ResourceInventory.Remove(resource);
                }
            }

            StartCoroutine(ProductionRoutine(recipe));
        }

        void StopProduction(Recipe recipe)
        {
            if (!ProductionRoutines.ContainsKey(recipe)) ProductionRoutines.Add(recipe, null);

            if (ProductionRoutines[recipe] != null)
            {
                StopCoroutine(ProductionRoutines[recipe]);
                ProductionRoutines[recipe] = null;
            }
        }

        void OnRecipeFinished(object sender, RecipeEventArgs e)
        {
            var targetInventory = TargetInventory(e.Recipe);
            targetInventory.Add(e.Recipe.Product);

            StartProduction(e.Recipe);
        }

        IEnumerator ProductionRoutine(Recipe recipe, int startPercentage = 0)
        {
            FeierabendReached += OnFeierabendReached;

            var percentageFinished = startPercentage;

            while (percentageFinished < 100)
            {
                var step = recipe.Time / 100 / Mathf.Max(AssignedEmployees(recipe), 1);

                yield return new WaitForSeconds(step);

                percentageFinished += 1;
                Debug.Log($"{recipe.name} is {percentageFinished}% done.");
            }

            RecipeFinished?.Invoke(this, new RecipeEventArgs(recipe));

            void OnFeierabendReached(object sender, EventArgs e)
            {
                Debug.Log("FEIERABEND");

            }
        }

        #region OdinInspector

        void OnValidate()
        {
            RecipeFinished -= OnRecipeFinished;
            RecipeFinished += OnRecipeFinished;
        }

        #endregion
    }
}