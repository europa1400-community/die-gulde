using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Gulde.Core.Tests
{
    public class ProductionTests
    {
        [Test]
        public void ShouldAssignEmployeeAndCreateProductionProgress()
        {
            var ingredient = ScriptableObject.CreateInstance<Item>();
            ingredient.itemFlags.Add(ItemFlag.Processable);
            
            var product = ScriptableObject.CreateInstance<Item>();
            product.itemFlags.Add(ItemFlag.Producible);
            
            var recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.ingredients.Add(ingredient);
            recipe.product = product;
            recipe.time = 1f;

            var gameObject = new GameObject();
            var productionComponent = gameObject.AddComponent<ProductionComponent>();
            var employeeComponent = gameObject.AddComponent<EmployeeComponent>();
            
            productionComponent.availableRecipes.Add(recipe);
            
            var wasAssigned = productionComponent.AddAssignment(new Assignment
            {
                recipe = recipe, employeeComponent = employeeComponent
            });
            
            Assert.IsTrue(wasAssigned);
            Assert.IsTrue(productionComponent.assignments.Exists(e => e.recipe == recipe && e.employeeComponent == employeeComponent));
            Assert.AreEqual(1, productionComponent.productionProgresses.Count(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldNotAssignEmployeeTwice()
        {
            var ingredient = ScriptableObject.CreateInstance<Item>();
            ingredient.itemFlags.Add(ItemFlag.Processable);
            
            var product = ScriptableObject.CreateInstance<Item>();
            product.itemFlags.Add(ItemFlag.Producible);
            
            var recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.ingredients.Add(ingredient);
            recipe.product = product;
            recipe.time = 1f;

            var gameObject = new GameObject();
            var productionComponent = gameObject.AddComponent<ProductionComponent>();
            var employeeComponent = gameObject.AddComponent<EmployeeComponent>();
            
            productionComponent.availableRecipes.Add(recipe);
            
            var wasAssigned1 = productionComponent.AddAssignment(new Assignment
            {
                recipe = recipe,
                employeeComponent = employeeComponent
            });
            
            Assert.IsTrue(wasAssigned1);
            
            var wasAssigned2 = productionComponent.AddAssignment(new Assignment
            {
                recipe = recipe,
                employeeComponent = employeeComponent
            });
            
            Assert.IsFalse(wasAssigned2);
            Assert.AreEqual(1, productionComponent.assignments.Count(e => e.recipe == recipe && e.employeeComponent == employeeComponent));
            Assert.AreEqual(1, productionComponent.productionProgresses.Count(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldAssignMultipleEmployees()
        {
            var ingredient = ScriptableObject.CreateInstance<Item>();
            ingredient.itemFlags.Add(ItemFlag.Processable);
            
            var product = ScriptableObject.CreateInstance<Item>();
            product.itemFlags.Add(ItemFlag.Producible);
            
            var recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.ingredients.Add(ingredient);
            recipe.product = product;
            recipe.time = 1f;

            var gameObject1 = new GameObject();
            var gameObject2 = new GameObject();
            
            var productionComponent = gameObject1.AddComponent<ProductionComponent>();
            productionComponent.availableRecipes.Add(recipe);
            
            var employeeComponent1 = gameObject1.AddComponent<EmployeeComponent>();
            var employeeComponent2 = gameObject2.AddComponent<EmployeeComponent>();
            
            var wasAssigned1 = productionComponent.AddAssignment(new Assignment
            {
                recipe = recipe,
                employeeComponent = employeeComponent1
            });
            var wasAssigned2 = productionComponent.AddAssignment(new Assignment
            {
                recipe = recipe,
                employeeComponent = employeeComponent2
            });
            
            Assert.IsTrue(wasAssigned1);
            Assert.IsTrue(wasAssigned2);
            Assert.IsTrue(productionComponent.assignments.Exists(e => e.recipe == recipe && e.employeeComponent == employeeComponent1));
            Assert.IsTrue(productionComponent.assignments.Exists(e => e.recipe == recipe && e.employeeComponent == employeeComponent2));
            Assert.AreEqual(1, productionComponent.productionProgresses.Count(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldUnassignEmployeeAndRemoveProductionProgress()
        {
            var ingredient = ScriptableObject.CreateInstance<Item>();
            ingredient.itemFlags.Add(ItemFlag.Processable);
            
            var product = ScriptableObject.CreateInstance<Item>();
            product.itemFlags.Add(ItemFlag.Producible);
            
            var recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.ingredients.Add(ingredient);
            recipe.product = product;
            recipe.time = 1f;

            var gameObject = new GameObject();
            var productionComponent = gameObject.AddComponent<ProductionComponent>();
            var employeeComponent = gameObject.AddComponent<EmployeeComponent>();
            
            productionComponent.availableRecipes.Add(recipe);
            
            var assignment = new Assignment
            {
                recipe = recipe,
                employeeComponent = employeeComponent
            };
            var wasAssigned = productionComponent.AddAssignment(assignment);
            
            Assert.IsTrue(wasAssigned);
            Assert.IsTrue(productionComponent.productionProgresses.Exists(e => e.recipe == recipe));

            var wasUnassigned = productionComponent.RemoveAssignment(assignment);
            
            Assert.IsTrue(wasUnassigned);
            Assert.IsFalse(productionComponent.productionProgresses.Exists(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldUnassignEmployeeAndNotRemoveProductionProgress()
        {
            var ingredient = ScriptableObject.CreateInstance<Item>();
            ingredient.itemFlags.Add(ItemFlag.Processable);
            
            var product = ScriptableObject.CreateInstance<Item>();
            product.itemFlags.Add(ItemFlag.Producible);
            
            var recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.ingredients.Add(ingredient);
            recipe.product = product;
            recipe.time = 1f;

            var gameObject1 = new GameObject();
            var gameObject2 = new GameObject();
            
            var productionComponent = gameObject1.AddComponent<ProductionComponent>();
            productionComponent.availableRecipes.Add(recipe);
            
            var employeeComponent1 = gameObject1.AddComponent<EmployeeComponent>();
            var employeeComponent2 = gameObject2.AddComponent<EmployeeComponent>();
            
            var assignment1 = new Assignment
            {
                recipe = recipe,
                employeeComponent = employeeComponent1
            };
            
            var assignment2 = new Assignment
            {
                recipe = recipe,
                employeeComponent = employeeComponent2
            };
            
            var wasAssigned1 = productionComponent.AddAssignment(assignment1);
            var wasAssigned2 = productionComponent.AddAssignment(assignment2);
            
            Assert.IsTrue(wasAssigned1);
            Assert.IsTrue(wasAssigned2);
            Assert.IsTrue(productionComponent.assignments.Exists(e => e.recipe == recipe && e.employeeComponent == employeeComponent1));
            Assert.IsTrue(productionComponent.assignments.Exists(e => e.recipe == recipe && e.employeeComponent == employeeComponent2));
            Assert.AreEqual(1, productionComponent.productionProgresses.Count(e => e.recipe == recipe));
            
            var wasUnassigned = productionComponent.RemoveAssignment(assignment1);
            
            Assert.IsTrue(wasUnassigned);
            Assert.IsFalse(productionComponent.assignments.Exists(e => e.recipe == recipe && e.employeeComponent == employeeComponent1));
            Assert.IsTrue(productionComponent.assignments.Exists(e => e.recipe == recipe && e.employeeComponent == employeeComponent2));
            Assert.AreEqual(1, productionComponent.productionProgresses.Count(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldNotUnassignUnassignedEmployee()
        {
            var recipe = ScriptableObject.CreateInstance<Recipe>();
            var gameObject = new GameObject();
            var productionComponent = gameObject.AddComponent<ProductionComponent>();
            var employeeComponent = gameObject.AddComponent<EmployeeComponent>();

            var wasUnassigned = productionComponent.RemoveAssignment(new Assignment
            {
                recipe = recipe,
                employeeComponent = employeeComponent
            });
            
            Assert.IsFalse(wasUnassigned);
            Assert.IsNull(productionComponent.productionProgresses.FirstOrDefault());
        }
    }
}