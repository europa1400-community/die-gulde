using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Gulde.Company;
using Gulde.Inventory;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gulde.Production
{
    [HideMonoScript]
    [RequireComponent(typeof(CompanyComponent))]
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

        [OdinSerialize]
        Dictionary<Recipe, int> ProductionPercentages { get; set; } = new Dictionary<Recipe, int>();

        [OdinSerialize]
        [ReadOnly]
        CompanyComponent Company { get; set; }

        public event EventHandler<ProductionEventArgs> RecipeFinished;

        public bool IsAssigned(EmployeeComponent employee) => Assignments.ContainsKey(employee);

        public bool IsAssignable(EmployeeComponent employee)
        {
            if (!IsAssigned(employee)) Assignments.Add(employee, null);
            var recipe = Assignments[employee];
            return !recipe || !recipe.IsExternal;
        }

        bool IsProducing(Recipe recipe) => ProductionRoutines.ContainsKey(recipe) && ProductionRoutines[recipe] != null;

        InventoryComponent TargetInventory(Recipe recipe) =>
            recipe.InventoryType == InventoryType.Product ? ProductInventory : ResourceInventory;

        public bool HasSlots(Recipe recipe)
        {
            var targetInventory = TargetInventory(recipe);
            return targetInventory.HasProduct(recipe.Product) || !targetInventory.IsFull;
        }

        public bool CanProduce(Recipe recipe)
        {
            foreach (var pair in recipe.Resources)
            {
                var resource = pair.Key;
                var amount = pair.Value;

                if (ResourceInventory.GetSupply(resource) < amount) return false;
            }

            return HasSlots(recipe);
        }

        public int AssignedEmployees(Recipe recipe) => Assignments.Count(e => e.Value == recipe);

        void Awake()
        {
            Company = GetComponent<CompanyComponent>();

            Locator.TimeComponent.Evening -= OnEvening;
            Company.Arrived -= OnEmployeeArrived;
            RecipeFinished -= OnRecipeFinished;

            Locator.TimeComponent.Evening += OnEvening;
            Company.Arrived += OnEmployeeArrived;
            RecipeFinished += OnRecipeFinished;
        }

        void OnEmployeeArrived(object sender, EmployeeEventArgs e)
        {
            if (!Assignments.ContainsKey(e.Employee)) return;

            var recipe = Assignments[e.Employee];
            ContinueProduction(recipe);
        }

        void OnEvening(object sender, EventArgs e)
        {
            var recipesToStop = new List<Recipe>();

            foreach (var pair in ProductionRoutines)
            {
                var recipe = pair.Key;
                var routine = pair.Value;

                if (routine == null)
                {
                    continue;
                }
                recipesToStop.Add(recipe);
            }

            foreach (var recipe in recipesToStop) StopProduction(recipe);
        }

        public void Assign(EmployeeComponent employee, Recipe recipe)
        {
            if (!IsAssignable(employee)) return;

            Unassign(employee);
            Assignments[employee] = recipe;

            if (!IsProducing(recipe)) StartProduction(recipe);
        }

        public void Unassign(EmployeeComponent employee)
        {
            if (!IsAssigned(employee)) Assignments.Add(employee, null);

            var recipe = Assignments[employee];
            if (!recipe) return;
            if (recipe.IsExternal) return;

            Assignments[employee] = null;

            if (AssignedEmployees(recipe) == 0) StopProduction(recipe);
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

            ProductionRoutines[recipe] = StartCoroutine(ProductionRoutine(recipe));
        }

        void ContinueProduction(Recipe recipe)
        {
            if (!recipe) return;
            if (!ProductionRoutines.ContainsKey(recipe)) ProductionRoutines.Add(recipe, null);
            if (!ProductionPercentages.ContainsKey(recipe)) ProductionPercentages.Add(recipe, 0);
            if (ProductionRoutines[recipe] != null) return;
            if (!HasSlots(recipe)) return;

            var targetInventory = TargetInventory(recipe);
            targetInventory.Register(recipe.Product);

            ProductionRoutines[recipe] = StartCoroutine(ProductionRoutine(recipe, ProductionPercentages[recipe]));
        }

        void StopProduction(Recipe recipe)
        {
            if (!ProductionRoutines.ContainsKey(recipe)) return;

            if (ProductionRoutines[recipe] != null)
            {
                StopCoroutine(ProductionRoutines[recipe]);
                ProductionRoutines[recipe] = null;
            }
        }

        void OnRecipeFinished(object sender, ProductionEventArgs e)
        {
            ProductionRoutines[e.Recipe] = null;

            var targetInventory = TargetInventory(e.Recipe);
            targetInventory.Add(e.Recipe.Product);

            if (e.Recipe.IsExternal)
            {
                foreach (var employee in e.Employees)
                {
                    Assignments[employee] = null;
                }
            }
            else StartProduction(e.Recipe);
        }

        IEnumerator ProductionRoutine(Recipe recipe, int startPercentage = 0)
        {
            if (!ProductionPercentages.ContainsKey(recipe)) ProductionPercentages.Add(recipe, 0);
            ProductionPercentages[recipe] = startPercentage;

            while (ProductionPercentages[recipe] < 100)
            {
                var step = recipe.Time / 100 / Mathf.Max(AssignedEmployees(recipe), 1);

                yield return new WaitForSeconds(step);

                ProductionPercentages[recipe] += 1;
                Debug.Log($"{recipe.name} is {ProductionPercentages[recipe]}% done.");
            }

            var employees = Assignments.Keys.Where(e => Assignments[e] == recipe).ToList();
            RecipeFinished?.Invoke(this, new ProductionEventArgs(recipe, employees));
        }

        #region OdinInspector

        void OnValidate()
        {
            if (!gameObject.scene.isLoaded) return;

            Company = GetComponent<CompanyComponent>();

            Locator.TimeComponent.Evening -= OnEvening;
            Company.Arrived -= OnEmployeeArrived;
            RecipeFinished -= OnRecipeFinished;

            Locator.TimeComponent.Evening += OnEvening;
            Company.Arrived += OnEmployeeArrived;
            RecipeFinished += OnRecipeFinished;
        }

        #endregion
    }
}