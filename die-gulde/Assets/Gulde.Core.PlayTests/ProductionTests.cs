using System.Collections;
using Gulde.Core.Inventory;
using Gulde.Core.Production;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

namespace Gulde.Core.PlayTests
{
    public class ProductionTests
    {
        ProductionComponent production;
        InventoryComponent ingredientInventory;
        InventoryComponent productInventory;
        EmployeeComponent employee;
        Recipe recipe;
        Item ingredient, product;

        [SetUp]
        public void Setup()
        {
            var productionGameObject = new GameObject();
            production = productionGameObject.AddComponent<ProductionComponent>();
            ingredientInventory = productionGameObject.AddComponent<InventoryComponent>();
            productInventory = productionGameObject.AddComponent<InventoryComponent>();
            
            var employeeGameObject = new GameObject();
            employee = employeeGameObject.AddComponent<EmployeeComponent>();
            
            ingredient = ScriptableObject.CreateInstance<Item>();
            ingredient.itemFlags.Add(ItemFlag.Processable);
            
            product = ScriptableObject.CreateInstance<Item>();
            product.itemFlags.Add(ItemFlag.Producible);
            
            recipe = ScriptableObject.CreateInstance<Recipe>();
            recipe.ingredients.Add(ingredient);
            recipe.product = product;
            recipe.time = 0.1f;

            ingredientInventory.slots.Add(new InventorySlot());
            ingredientInventory.stackSize = 1;
            
            productInventory.slots.Add(new InventorySlot());
            productInventory.stackSize = 1;
            
            production.ingredientInventory = ingredientInventory;
            production.productInventory = productInventory;
            production.availableRecipes.Add(recipe);
        }

        [UnityTest]
        public IEnumerator ShouldProduce()
        {
            ingredientInventory.Add(ingredient);
            
            production.AddAssignment(new Assignment
            {
                recipe = recipe, employee = employee
            });
            
            Assert.IsTrue(ingredientInventory.HasItem(ingredient));
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe));
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
            
            yield return new WaitForSeconds(0.05f);
            
            Assert.IsTrue(ingredientInventory.HasItem(ingredient));
            Assert.IsFalse(productInventory.HasItem(product));
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe));
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
            
            yield return new WaitForSeconds(0.1f);
            
            Assert.IsFalse(ingredientInventory.HasItem(ingredient));
            Assert.IsTrue(productInventory.HasItem(product));
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe));
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
        }

        [UnityTest]
        public IEnumerator ShouldNotProduceWithoutIngredients()
        {
            production.AddAssignment(new Assignment
            {
                recipe = recipe, employee = employee
            });
            
            var productionProgress = production.productionProgresses.First(e => e.recipe == recipe);
            
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe));
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
            Assert.AreEqual(0f, productionProgress.progress);
            
            yield return new WaitForSeconds(0.05f);
            
            Assert.IsFalse(productInventory.HasItem(product));
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
            Assert.AreEqual(0f, productionProgress.progress);
            
            yield return new WaitForSeconds(0.1f);
            
            Assert.IsFalse(productInventory.HasItem(product));
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
            Assert.AreEqual(0f, productionProgress.progress);
        }

        [UnityTest]
        public IEnumerator ShouldNotFinishProductionWithoutSpaceForProduct()
        {
            var item = ScriptableObject.CreateInstance<Item>();
            item.itemFlags.Add(ItemFlag.Producible);
            
            ingredientInventory.Add(ingredient);
            
            production.AddAssignment(new Assignment
            {
                recipe = recipe, employee = employee
            });
            
            Assert.IsTrue(ingredientInventory.HasItem(ingredient));
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe));
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
            
            yield return new WaitForSeconds(0.05f);

            var slot = productInventory.slots.First();
            productInventory.Register(item, slot);
            
            Assert.IsTrue(ingredientInventory.HasItem(ingredient));
            Assert.IsFalse(productInventory.HasItem(product));
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe));
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
            
            yield return new WaitForSeconds(0.1f);
            
            Assert.IsTrue(ingredientInventory.HasItem(ingredient));
            Assert.IsFalse(productInventory.HasItem(product));
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe));
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
        }

        [UnityTest]
        public IEnumerator ShouldForageAndRemoveAssignment()
        {
            recipe.ingredients.Clear();
            product.itemFlags.Add(ItemFlag.Forageable);
            
            production.AddAssignment(new Assignment
            {
                recipe = recipe, employee = employee
            });
            
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe));
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
            
            yield return new WaitForSeconds(0.05f);
            
            Assert.IsFalse(productInventory.HasItem(product));
            Assert.IsTrue(production.assignments.Exists(e => e.recipe == recipe));
            Assert.IsTrue(production.productionProgresses.Exists(e => e.recipe == recipe));
            
            yield return new WaitForSeconds(0.1f);
            
            Assert.IsTrue(productInventory.HasItem(product));
            Assert.IsFalse(production.assignments.Exists(e => e.recipe == recipe));
            Assert.IsFalse(production.productionProgresses.Exists(e => e.recipe == recipe));
        }
    }
}