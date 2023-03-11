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
                    FinishProduction(productionProgress);
                }
            }
        }

        public bool AddAssignment(Recipe recipe, EmployeeComponent employeeComponent)
        {
            if (assignments.Exists(e => e.employeeComponent == employeeComponent))
            {
                return false;
            }
            
            if (!assignments.Exists(e => e.recipe == recipe))
            {
                StartProduction(recipe);
            }
            
            var assignment = new Assignment
            {
                recipe = recipe,
                employeeComponent = employeeComponent
            };
            
            assignments.Add(assignment);
            return true;
        }

        public bool RemoveAssignment(EmployeeComponent employeeComponent)
        {
            var assignment = assignments.FirstOrDefault(e => e.employeeComponent == employeeComponent);
            if (assignment == default(Assignment))
            {
                return false;
            }
            
            assignments.Remove(assignment);
            
            if (!assignments.Exists(e => e.recipe == assignment.recipe))
            {
                productionProgresses.RemoveAll(e => e.recipe == assignment.recipe);
            }

            return true;
        }

        bool RemoveAssignment(Assignment assignment)
        {
            if (assignment == default(Assignment))
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
            //TODO: Remove ingredients from ingredient inventory
                    
            var productionProgress = new ProductionProgress
            {
                recipe = recipe,
                progress = 0
            };
            
            productionProgresses.Add(productionProgress);
        }

        void FinishProduction(ProductionProgress productionProgress)
        {
            var recipe = productionProgress.recipe;
            
            //TODO: Add product to product inventory

            if (!recipe.product.itemFlags.Contains(ItemFlag.Forageable))
            {
                StartProduction(recipe);
                return;
            }
            
            var finishedAssignments = assignments.Where(e => e.recipe == recipe);

            foreach (var finishedAssignment in finishedAssignments)
            {
                RemoveAssignment(finishedAssignment);
            }

            productionProgresses.Remove(productionProgress);
        }
        
        void CancelProduction(Recipe recipe)
        {
            productionProgresses.RemoveAll(e => e.recipe == recipe);
            
            //TODO: Add ingredients to ingredient inventory
        }
    }
}