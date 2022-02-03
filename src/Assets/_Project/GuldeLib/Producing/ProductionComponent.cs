using System;
using System.Collections.Generic;
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
    public class ProductionComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public InventoryComponent ResourceInventory => GetComponent<InventoryComponent>();

        [ShowInInspector]
        [BoxGroup("Info")]
        public InventoryComponent ProductInventory => GetComponents<InventoryComponent>().ElementAtOrDefault(1);

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        CompanyComponent Company => GetComponent<CompanyComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        ExchangeComponent Exchange => GetComponent<ExchangeComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public AssignmentComponent Assignment => GetComponent<AssignmentComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public ProductionRegistryComponent Registry => GetComponent<ProductionRegistryComponent>();

        public event EventHandler<InitializedEventArgs> Initialized;

        void Start()
        {
            Initialized?.Invoke(this, new InitializedEventArgs());
        }

        public bool HasProductSlots(Recipe recipe)
        {
            var targetInventory = Exchange.GetTargetInventory(recipe.Product);
            return targetInventory.CanRegisterItem(recipe.Product);
        }

        public bool HasResources(Recipe recipe, int amount = 1) =>
            recipe.Resources.All(pair => pair.Value * amount <= ResourceInventory.GetSupply(pair.Key));

        public bool CanProduce(Recipe recipe)
        {
            var hasResources = HasResources(recipe);
            var hasProductSlots = HasProductSlots(recipe);
            if (!hasResources) this.Log($"Cannot produce recipe {recipe}: Not enough resources");
            if (!hasProductSlots) this.Log($"Cannot produce recipe {recipe}: Not enough product slots");

            return hasResources && hasProductSlots;
        }

        public void OnItemAdded(object sender, InventoryComponent.ItemEventArgs e)
        {
            foreach (var recipe in Registry.HaltedRecipes)
            {
                if (!recipe.Resources.ContainsKey(e.Item)) continue;

                this.Log($"Production restarting halted production of {recipe}");

                StartProduction(recipe);
            }
        }

        public void OnEmployeeAssigned(object sender, AssignmentComponent.AssignmentEventArgs e)
        {
            if (Registry.IsProducing(e.Recipe)) return;

            this.Log($"Production will start after first assignment for {e.Recipe}");
            StartProduction(e.Recipe);
        }

        public void OnEmployeeUnassigned(object sender, AssignmentComponent.AssignmentEventArgs e)
        {
            if (Assignment.IsAssigned(e.Recipe)) return;

            this.Log($"Production will start after last unassignment for {e.Recipe}");
            StopProduction(e.Recipe);
        }

        public void OnRecipeFinished(object sender, ProductionEventArgs e)
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

        public void OnEmployeeArrived(object sender, CompanyComponent.EmployeeArrivedEventArgs e)
        {
            var recipe = Assignment.GetRecipe(e.Employee);
            if (!recipe)
            {
                this.Log($"Production will not continue: Recipe was null");
                return;
            }

            StartProduction(recipe);
        }

        public void OnEvening(object sender, EventArgs e)
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

        public class InitializedEventArgs : EventArgs
        {
        }

        public class ProductionEventArgs : EventArgs
        {
            public ProductionEventArgs(Recipe recipe, List<EmployeeComponent> employees)
            {
                Recipe = recipe;
                Employees = employees;
            }

            public Recipe Recipe { get; }

            public List<EmployeeComponent> Employees { get; }
        }
    }
}