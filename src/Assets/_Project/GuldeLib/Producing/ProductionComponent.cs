using System;
using System.Linq;
using GuldeLib.Companies;
using GuldeLib.Companies.Employees;
using GuldeLib.Economy;
using GuldeLib.Inventories;
using GuldeLib.TypeObjects;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Producing
{
    [RequireComponent(typeof(CompanyComponent))]
    [RequireComponent(typeof(AssignmentComponent))]
    [RequireComponent(typeof(ProductionRegistryComponent))]
    public class ProductionComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public InventoryComponent ResourceInventory => this.GetCachedComponent<InventoryComponent>();

        [ShowInInspector]
        [BoxGroup("Info")]
        public InventoryComponent ProductInventory => this.GetCachedComponent<InventoryComponent>(1);

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        CompanyComponent Company => this.GetCachedComponent<CompanyComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        ExchangeComponent Exchange => this.GetCachedComponent<ExchangeComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public AssignmentComponent Assignment => this.GetCachedComponent<AssignmentComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public ProductionRegistryComponent Registry => this.GetCachedComponent<ProductionRegistryComponent>();

        public bool HasProductSlots(Recipe recipe)
        {
            var targetInventory = Exchange.GetTargetInventory(recipe.Product);
            return targetInventory.CanRegisterItem(recipe.Product);
        }

        public bool HasResources(Recipe recipe, int amount = 1) =>
            recipe.Resources.All(pair => pair.Value * amount <= ResourceInventory.GetSupply(pair.Key));

        public bool CanProduce(Recipe recipe)
            => HasResources(recipe) && HasProductSlots(recipe);

        void Awake()
        {
            this.Log("Production initializing");
        }

        void Start()
        {
            Assignment.Assigned += OnEmployeeAssigned;
            Assignment.Unassigned += OnEmployeeUnassigned;
            Company.EmployeeArrived += OnEmployeeArrived;
            Registry.RecipeFinished += OnRecipeFinished;
            ResourceInventory.Added += OnItemAdded;
            if (Locator.Time) Locator.Time.Evening += OnEvening;
        }

        void OnItemAdded(object sender, ItemEventArgs e)
        {
            foreach (var recipe in Registry.HaltedRecipes)
            {
                if (!recipe.Resources.ContainsKey(e.Item)) continue;

                this.Log($"Production restarting halted production of {recipe}");

                StartProduction(recipe);
            }
        }

        void OnEmployeeAssigned(object sender, AssignmentEventArgs e)
        {
            if (Registry.IsProducing(e.Recipe)) return;

            this.Log($"Production will start after first assignment for {e.Recipe}");
            StartProduction(e.Recipe);
        }

        void OnEmployeeUnassigned(object sender, AssignmentEventArgs e)
        {
            if (Assignment.IsAssigned(e.Recipe)) return;

            this.Log($"Production will start after last unassignment for {e.Recipe}");
            StopProduction(e.Recipe);
        }

        void OnRecipeFinished(object sender, ProductionEventArgs e)
        {
            this.Log($"Production finished for {e.Recipe}");

            StopProduction(e.Recipe);

            var targetInventory = Exchange.GetTargetInventory(e.Recipe.Product);
            targetInventory.Add(e.Recipe.Product);

            if (e.Recipe.IsExternal)
            {
                this.Log($"Production stopping external recipe {e.Recipe}");

                foreach (var employee in e.Employees)
                {
                    Assignment.Unassign(employee);
                }
            }
            else
            {
                this.Log($"Production restarting for {e.Recipe}");
                StartProduction(e.Recipe);
            }
        }

        void OnEmployeeArrived(object sender, EmployeeArrivedEventArgs e)
        {
            var recipe = Assignment.GetRecipe(e.Employee);
            if (!recipe)
            {
                this.Log($"Production will not restart: Recipe was null", LogType.Warning);
                return;
            }

            StartProduction(recipe);
        }

        void OnEvening(object sender, EventArgs e)
        {
            this.Log($"Production halting productions at evening");
            Registry.StopProductionRoutines();
        }

        void StartProduction(Recipe recipe)
        {
            this.Log($"Production starting for {recipe}");

            if (!recipe)
            {
                this.Log($"Production will not start: Recipe was null", LogType.Warning);
                return;
            }

            if (Registry.IsProducing(recipe))
            {
                this.Log($"Production will not start: Recipe was already being produced", LogType.Warning);
                return;
            }

            if (!CanProduce(recipe) && !Registry.HasProgress(recipe))
            {
                this.Log($"Production will not start: Recipe can not be produced", LogType.Warning);
                return;
            }

            var targetInventory = Exchange.GetTargetInventory(recipe.Product);
            targetInventory.Register(recipe.Product);

            if (!Registry.HasProgress(recipe))
            {
                this.Log($"Production removing resources for started recipe {recipe}");
                ResourceInventory.RemoveResources(recipe);
            }

            Registry.StartProductionRoutine(recipe);
        }

        void StopProduction(Recipe recipe)
        {
            this.Log($"Production stopping for {recipe}");

            if (!recipe)
            {
                this.Log($"Production will not stop: Recipe was null", LogType.Warning);
                return;
            }

            if (!Registry.IsProducing(recipe))
            {
                this.Log($"Production will not stop: Recipe was not being produced", LogType.Warning);
                return;
            }

            if (Registry.HasProgress(recipe))
            {
                this.Log($"Production adding resources for halted recipe {recipe}");
                ResourceInventory.AddResources(recipe);
            }

            if (!Assignment.IsAssigned(recipe))
            {
                this.Log($"Production resetting progress for recipe {recipe} without assignments");
                Registry.ResetProgress(recipe);
            }

            Registry.StopProductionRoutine(recipe);
        }
    }
}