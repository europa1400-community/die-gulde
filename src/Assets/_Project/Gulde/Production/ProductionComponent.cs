using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Gulde.Company;
using Gulde.Company.Employees;
using Gulde.Economy;
using Gulde.Inventory;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gulde.Production
{
    [RequireComponent(typeof(CompanyComponent))]
    public class ProductionComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public InventoryComponent ResourceInventory { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public InventoryComponent ProductInventory { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        CompanyComponent Company { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        ExchangeComponent Exchange { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public AssignmentComponent Assignment { get; private set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public ProductionRegistryComponent Registry { get; private set; }

        public bool HasSlots(Recipe recipe)
        {
            var targetInventory = Exchange.GetTargetInventory(recipe.Product);
            return targetInventory.IsRegistered(recipe.Product) || !targetInventory.IsFull;
        }

        public bool HasResources(Recipe recipe, int amount = 1) =>
            recipe.Resources.All(pair => pair.Value * amount <= ResourceInventory.GetSupply(pair.Key));

        public bool CanProduce(Recipe recipe) =>
            HasResources(recipe) && HasSlots(recipe);

        void Awake()
        {
            this.Log("Production initializing");

            Assignment = GetComponent<AssignmentComponent>();
            Registry = GetComponent<ProductionRegistryComponent>();
            Company = GetComponent<CompanyComponent>();
            Exchange = GetComponent<ExchangeComponent>();
            var inventories = GetComponents<InventoryComponent>();
            ResourceInventory = inventories[0];
            ProductInventory = inventories[1];

            if (Locator.Time) Locator.Time.Evening += OnEvening;
            Assignment.Assigned += OnEmployeeAssigned;
            Assignment.Unassigned += OnEmployeeUnassigned;
            Company.EmployeeArrived += OnEmployeeArrived;
            Registry.RecipeFinished += OnRecipeFinished;
            ResourceInventory.Added += OnItemAdded;
        }

        void OnItemAdded(object sender, ItemEventArgs e)
        {
            foreach (var recipe in Registry.HaltedRecipes)
            {
                this.Log($"Production restarting halted production of {recipe}");

                StartProduction(recipe);
            }
        }

        void OnEmployeeAssigned(object sender, AssignmentEventArgs e)
        {
            if (Registry.IsProducing(e.Recipe)) return;

            StartProduction(e.Recipe);
        }

        void OnEmployeeUnassigned(object sender, AssignmentEventArgs e)
        {
            if (!Assignment.IsAssigned(e.Recipe)) return;

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

        void OnEmployeeArrived(object sender, EmployeeEventArgs e)
        {
            var recipe = Assignment.GetRecipe(e.Employee);
            if (!recipe) return;

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

            if (!recipe) return;

            if (Registry.IsProducing(recipe)) return;
            if (!CanProduce(recipe) && !Registry.HasProgress(recipe)) return;

            var targetInventory = Exchange.GetTargetInventory(recipe.Product);
            targetInventory.Register(recipe.Product);

            if (!Registry.HasProgress(recipe)) targetInventory.RemoveResources(recipe);

            Registry.StartProductionRoutine(recipe);
        }

        void StopProduction(Recipe recipe)
        {
            this.Log($"Production stopping for {recipe}");

            if (!recipe) return;
            if (!Registry.IsProducing(recipe)) return;

            ResourceInventory.AddResources(recipe);

            Registry.StopProductionRoutine(recipe);
        }
    }
}