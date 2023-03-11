using System.Linq;
using Gulde.Core.Inventory;
using Gulde.Core.Production;
using NUnit.Framework;
using UnityEngine;

namespace Gulde.Core.Tests
{
    public class ProductionTests
    {
        ProductionComponent production;
        InventoryComponent ingredientInventory;
        InventoryComponent productInventory;
        EmployeeComponent employee;
        Recipe recipe;

        [SetUp]
        public void Setup()
        {
            var productionGameObject = new GameObject();
            production = productionGameObject.AddComponent<ProductionComponent>();
            ingredientInventory = productionGameObject.AddComponent<InventoryComponent>();
            productInventory = productionGameObject.AddComponent<InventoryComponent>();
            
            var employeeGameObject = new GameObject();
            employee = employeeGameObject.AddComponent<EmployeeComponent>();
            
            var ingredient = ScriptableObject.CreateInstance<Item>();
            ingredient.itemFlags.Add(ItemFlag.Processable);
            
            var product = ScriptableObject.CreateInstance<Item>();
            product.itemFlags.Add(ItemFlag.Producible);
            
            recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.ingredients.Add(ingredient);
            recipe.product = product;
            recipe.time = 1f;

            ingredientInventory.slots.Add(new InventorySlot());
            ingredientInventory.stackSize = 1;
            
            productInventory.slots.Add(new InventorySlot());
            productInventory.stackSize = 1;
            
            production.ingredientInventory = ingredientInventory;
            production.productInventory = productInventory;
            production.availableRecipes.Add(recipe);
        }
        
        [Test]
        public void ShouldAssignEmployeeAndCreateProductionProgress()
        {
            var wasAssigned = production.AddAssignment(new Assignment
            {
                recipe = recipe, employee = employee
            });
            
            Assert.IsTrue(wasAssigned);
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe && e.employee == employee));
            Assert.AreEqual(1, production.productionProgresses.Count(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldNotAssignNullAssignment()
        {
            var wasAssigned = production.AddAssignment(null);
            
            Assert.IsFalse(wasAssigned);
            Assert.AreEqual(0, production.assignments.Count);
        }
        
        [Test]
        public void ShouldNotAssignUnavailableRecipe()
        {
            var wasAssigned = production.AddAssignment(new Assignment
            {
                recipe = null, employee = employee
            });
            
            Assert.IsFalse(wasAssigned);
            Assert.AreEqual(0, production.assignments.Count);
        }
        
        [Test]
        public void ShouldNotAssignNullEmployee()
        {
            var wasAssigned = production.AddAssignment(new Assignment
            {
                recipe = recipe, employee = null
            });
            
            Assert.IsFalse(wasAssigned);
            Assert.AreEqual(0, production.assignments.Count);
        }
        
        [Test]
        public void ShouldNotAssignWithoutSpaceForProduct()
        {
            productInventory.slots.Clear();
            
            var wasAssigned = production.AddAssignment(new Assignment
            {
                recipe = recipe, employee = employee
            });
            
            Assert.IsFalse(wasAssigned);
            Assert.AreEqual(0, production.assignments.Count);
        }
        
        [Test]
        public void ShouldNotAssignEmployeeTwice()
        {
            var wasAssigned1 = production.AddAssignment(new Assignment
            {
                recipe = recipe, employee = employee
            });
            
            Assert.IsTrue(wasAssigned1);
            
            var wasAssigned2 = production.AddAssignment(new Assignment
            {
                recipe = recipe, employee = employee
            });
            
            Assert.IsFalse(wasAssigned2);
            Assert.AreEqual(1, production.assignments.Count(e => e.recipe == recipe && e.employee == employee));
            Assert.AreEqual(1, production.productionProgresses.Count(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldAssignMultipleEmployees()
        {
            var employeeGameObject2 = new GameObject();
            var employee2 = employeeGameObject2.AddComponent<EmployeeComponent>();
            
            var wasAssigned1 = production.AddAssignment(new Assignment
            {
                recipe = recipe,
                employee = employee
            });
            
            var wasAssigned2 = production.AddAssignment(new Assignment
            {
                recipe = recipe,
                employee = employee2
            });
            
            Assert.IsTrue(wasAssigned1);
            Assert.IsTrue(wasAssigned2);
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe && e.employee == employee));
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe && e.employee == employee2));
            Assert.AreEqual(1, production.productionProgresses.Count(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldUnassignAssignmentAndRemoveProductionProgress()
        {
            var assignment = new Assignment
            {
                recipe = recipe,
                employee = employee
            };
            var wasAssigned = production.AddAssignment(assignment);
            
            Assert.IsTrue(wasAssigned);
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));

            var wasUnassigned = production.RemoveAssignment(assignment);
            
            Assert.IsTrue(wasUnassigned);
            Assert.IsFalse(production.productionProgresses.Exists(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldUnassignEmployeeAndRemoveProductionProgress()
        {
            var assignment = new Assignment
            {
                recipe = recipe,
                employee = employee
            };
            var wasAssigned = production.AddAssignment(assignment);
            
            Assert.IsTrue(wasAssigned);
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));

            var wasUnassigned = production.RemoveAssignment(employee);
            
            Assert.IsTrue(wasUnassigned);
            Assert.IsFalse(production.productionProgresses.Exists(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldUnassignAssignmentAndNotRemoveProductionProgress()
        {
            var gameObject = new GameObject();
            var employee2 = gameObject.AddComponent<EmployeeComponent>();
            
            var assignment1 = new Assignment
            {
                recipe = recipe,
                employee = employee
            };
            
            var assignment2 = new Assignment
            {
                recipe = recipe,
                employee = employee2
            };
            
            var wasAssigned1 = production.AddAssignment(assignment1);
            var wasAssigned2 = production.AddAssignment(assignment2);
            
            Assert.IsTrue(wasAssigned1);
            Assert.IsTrue(wasAssigned2);
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe && e.employee == employee));
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe && e.employee == employee2));
            Assert.AreEqual(1, production.productionProgresses.Count(e => e.recipe == recipe));
            
            var wasUnassigned = production.RemoveAssignment(assignment1);
            
            Assert.IsTrue(wasUnassigned);
            Assert.IsFalse(production.assignments.Exists(e => e.recipe == recipe && e.employee == employee));
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe && e.employee == employee2));
            Assert.AreEqual(1, production.productionProgresses.Count(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldNotUnassignNullAssignment()
        {
            var assignment = new Assignment
            {
                recipe = recipe,
                employee = employee
            };
            
            var wasAssigned = production.AddAssignment(assignment);
            
            Assert.IsTrue(wasAssigned);
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));

            var wasUnassigned = production.RemoveAssignment(assignment = null);
            
            Assert.IsFalse(wasUnassigned);
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldNotUnassignUnassignedAssignment()
        {
            var gameObject2 = new GameObject();
            var employee2 = gameObject2.AddComponent<EmployeeComponent>();
            
            var assignment = new Assignment
            {
                recipe = recipe,
                employee = employee
            };
            
            var assignment2 = new Assignment
            {
                recipe = recipe,
                employee = employee2
            };
            
            var wasAssigned = production.AddAssignment(assignment);
            
            Assert.IsTrue(wasAssigned);
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));

            var wasUnassigned = production.RemoveAssignment(assignment2);
            
            Assert.IsFalse(wasUnassigned);
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldNotUnassignNullEmployee()
        {
            var assignment = new Assignment
            {
                recipe = recipe,
                employee = employee
            };
            var wasAssigned = production.AddAssignment(assignment);
            
            Assert.IsTrue(wasAssigned);
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));

            var wasUnassigned = production.RemoveAssignment(employee = null);
            
            Assert.IsFalse(wasUnassigned);
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
        }
        
        [Test]
        public void ShouldNotUnassignUnassignedEmployee()
        {
            var wasUnassigned = production.RemoveAssignment(employee);
            
            Assert.IsFalse(wasUnassigned);
            Assert.IsNull(production.productionProgresses.FirstOrDefault());
        }
    }
}