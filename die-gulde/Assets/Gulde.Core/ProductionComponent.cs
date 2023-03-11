using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gulde.Core
{
    public class ProductionComponent : MonoBehaviour
    {
        [SerializeField]
        public InventoryComponent ingredientInventory;
        
        [SerializeField]
        public InventoryComponent productInventory;

        [SerializeField]
        public List<ProductionProgress> productionProgresses = new();
        
        [SerializeField]
        public List<Recipe> availableRecipes = new();
        
        [SerializeField]
        public List<Assignment> assignments = new();

        void FixedUpdate()
        {
            foreach (var recipe in availableRecipes)
            {
                var assignmentCount = assignments.Count(e => e.recipe == recipe);
                if (assignmentCount == 0)
                {
                    continue;
                }
                
                var productionProgress = productionProgresses.First(e => e.recipe == recipe);
                productionProgress.progress += assignmentCount * Time.fixedDeltaTime / recipe.time;
                
                if (productionProgress.progress >= 1.0f)
                {
                    FinishProduction(recipe);
                }
            }
        }

        public bool AddAssignment(Assignment assignment)
        {
            if (assignments.Exists(e => e.employeeComponent == assignment.employeeComponent))
            {
                return false;
            }
            
            assignments.Add(assignment);
            StartProduction(assignment.recipe);
            
            return true;
        }

        public bool RemoveAssignment(Assignment assignment)
        {
            if (assignment is null)
            {
                return false;
            }
            
            if (!assignments.Contains(assignment))
            {
                return false;
            }
            
            assignments.Remove(assignment);
            
            if (!assignments.Exists(e => e.recipe == assignment.recipe))
            {
                CancelProduction(assignment.recipe);
            }

            return true;
        }

        void StartProduction(Recipe recipe)
        {
            if (productionProgresses.Exists(e => e.recipe == recipe))
            {
                return;
            }
                    
            //TODO: Remove ingredients from ingredient inventory
            
            var productionProgress = new ProductionProgress
            {
                recipe = recipe,
                progress = 0
            };
            
            productionProgresses.Add(productionProgress);
        }
        
        void CancelProduction(Recipe recipe)
        {
            productionProgresses.RemoveAll(e => e.recipe == recipe);
            
            //TODO: Add ingredients to ingredient inventory
        }

        void FinishProduction(Recipe recipe)
        {
            var productionProgress = productionProgresses.FirstOrDefault(e => e.recipe == recipe);
            
            if (productionProgress is null)
            {
                return;
            }
            
            //TODO: Add product to product inventory
            
            if (!recipe.product.itemFlags.Contains(ItemFlag.Forageable))
            {
                productionProgress.Reset();
                return;
            }
            
            var finishedAssignments = assignments.Where(e => e.recipe == recipe);

            foreach (var finishedAssignment in finishedAssignments)
            {
                RemoveAssignment(finishedAssignment);
            }

            productionProgresses.Remove(productionProgress);
        }
    }
}