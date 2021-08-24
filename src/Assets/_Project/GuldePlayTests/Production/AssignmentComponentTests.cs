using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gulde.Builders;
using Gulde.Company;
using Gulde.Company.Employees;
using Gulde.Economy;
using Gulde.Entities;
using Gulde.Inventory;
using Gulde.Production;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Production
{
    public class AssignmentComponentTests
    {
        CompanyBuilder CompanyBuilder { get; set; }
        PlayerBuilder PlayerBuilder { get; set; }

        GameObject CompanyObject { get; set; }
        GameObject PlayerObject { get; set; }

        CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();
        AssignmentComponent Assignment => CompanyObject.GetComponent<AssignmentComponent>();
        EntityRegistryComponent EntityRegistry => CompanyObject.GetComponent<EntityRegistryComponent>();
        InventoryComponent ResourceInventory => CompanyObject.GetComponents<InventoryComponent>()[0];
        InventoryComponent ProductionInventory => CompanyObject.GetComponents<InventoryComponent>()[1];
        ProductionComponent Production => CompanyObject.GetComponent<ProductionComponent>();
        ProductionRegistryComponent ProductionRegistry => CompanyObject.GetComponent<ProductionRegistryComponent>();
        WealthComponent Owner => PlayerObject.GetComponent<WealthComponent>();

        EmployeeComponent AssignedEmployee { get; set; }
        Recipe AssignedRecipe { get; set; }

        bool AssignedFlag { get; set; }
        bool UnassignedFlag { get; set; }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            var resource = An.Item.WithName("Resource").WithItemType(ItemType.Resource).WithMeanPrice(0f)
                .WithMinPrice(0f).WithMeanSupply(0).Build();
            var product = An.Item.WithName("Product").WithItemType(ItemType.Product).WithMeanPrice(0).WithMinPrice(0)
                .WithMeanSupply(0).Build();

            var resources = new Dictionary<Item, int> { { resource, 1 } };
            var recipe = A.Recipe
                .WithName("recipe0")
                .WithResources(resources)
                .WithProduct(product)
                .WithExternality(false)
                .WithTime(1)
                .Build();
            var recipes = new HashSet<Recipe> { recipe };

            PlayerBuilder = A.Player;
            yield return PlayerBuilder.Build();
            PlayerObject = PlayerBuilder.PlayerObject;

            CompanyBuilder = A.Company.WithOwner(Owner).WithSlots(5, 3).WithEmployees(1).WithRecipes(recipes);
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            AssignedEmployee = null;
            AssignedRecipe = null;
            AssignedFlag = false;
            UnassignedFlag = false;
        }

        [UnityTest]
        public IEnumerator ShouldAssign()
        {
            yield return CompanyBuilder.Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var recipe = ProductionRegistry.Recipes.ElementAt(0);
            var employee = Company.Employees.ElementAt(0);

            Assignment.Assigned += OnAssigned;

            Assignment.Assign(employee, recipe);

            Assert.True(Assignment.IsAssigned(employee));
            Assert.True(Assignment.IsAssigned(recipe));
            Assert.AreEqual(employee, AssignedEmployee);
            Assert.AreEqual(recipe, AssignedRecipe);
            Assert.True(AssignedFlag);
        }

        [UnityTest]
        public IEnumerator ShouldReassignFromInternal()
        {
            var resources = new Dictionary<Item, int>();
            var product = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var externalRecipe = A.Recipe.WithResources(resources).WithProduct(product).WithExternality(true)
                .WithTime(1)
                .Build();

            yield return CompanyBuilder.WithRecipe(externalRecipe).Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var internalRecipe = ProductionRegistry.Recipes.ElementAt(0);
            var employee = Company.Employees.ElementAt(0);
            Assignment.Assign(employee, internalRecipe);

            Assert.True(Assignment.IsAssigned(internalRecipe));
            Assert.AreEqual(internalRecipe, Assignment.GetRecipe(employee));

            Assignment.Assign(employee, externalRecipe);

            Assert.True(Assignment.IsAssigned(externalRecipe));
            Assert.AreEqual(externalRecipe, Assignment.GetRecipe(employee));
        }

        [UnityTest]
        public IEnumerator ShouldNotAssignUnknownEmployee()
        {
            yield return CompanyBuilder.Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            var otherCompanyBuilder = A.Company.WithOwner(Owner).WithEmployees(1);
            yield return otherCompanyBuilder.Build();
            var otherCompanyObject = otherCompanyBuilder.CompanyObject;

            var otherCompany = otherCompanyObject.GetComponent<CompanyComponent>();
            var otherEmployee = otherCompany.Employees.ElementAt(0);

            Assignment.Assign(otherEmployee, recipe);

            Assert.False(Assignment.IsAssigned(otherEmployee));
            Assert.False(Assignment.IsAssigned(recipe));
            Assert.AreEqual(0, Assignment.AssignmentCount());
        }

        [UnityTest]
        public IEnumerator ShouldNotAssignExternalEmployee()
        {
            var resources = new Dictionary<Item, int>();
            var product = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var externalRecipe = A.Recipe.WithResources(resources).WithProduct(product).WithExternality(true).WithTime(1)
                .Build();

            yield return CompanyBuilder.WithRecipe(externalRecipe).Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var employee = Company.Employees.ElementAt(0);
            Assignment.Assign(employee, externalRecipe);

            Assert.AreEqual(externalRecipe, Assignment.GetRecipe(employee));
            Assert.False(Assignment.IsAssignable(employee));

            var otherRecipe = ProductionRegistry.Recipes.ElementAt(0);
            Assignment.Assign(employee, otherRecipe);

            Assert.AreEqual(externalRecipe, Assignment.GetRecipe(employee));
            Assert.True(Assignment.IsAssigned(externalRecipe));
            Assert.False(Assignment.IsAssigned(otherRecipe));
        }

        [UnityTest]
        public IEnumerator ShouldNotAssignUnavailableEmployee()
        {
            yield return CompanyBuilder.Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var employee = Company.Employees.ElementAt(0);
            var entity = employee.GetComponent<EntityComponent>();
            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            EntityRegistry.Unregister(entity);
            Assignment.Assign(employee, recipe);

            Assert.False(Assignment.IsAssigned(recipe));
            Assert.False(Assignment.IsAssigned(employee));
            Assert.IsNull(Assignment.GetRecipe(employee));
        }

        [UnityTest]
        public IEnumerator ShouldAssignAll()
        {
            yield return CompanyBuilder.WithEmployees(3).Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var employee1 = Company.Employees.ElementAt(0);
            var employee2 = Company.Employees.ElementAt(1);
            var employee3 = Company.Employees.ElementAt(2);
            var entity2 = employee2.GetComponent<EntityComponent>();
            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            EntityRegistry.Unregister(entity2);
            Assignment.AssignAll(recipe);

            Assert.True(Assignment.IsAssigned(employee1));
            Assert.False(Assignment.IsAssigned(employee2));
            Assert.True(Assignment.IsAssigned(employee3));
        }

        [UnityTest]
        public IEnumerator ShouldUnassign()
        {
            yield return CompanyBuilder.Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var employee = Company.Employees.ElementAt(0);
            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            Assignment.Assign(employee, recipe);

            Assert.True(Assignment.IsAssigned(employee));
            Assert.True(Assignment.IsAssigned(recipe));

            Assignment.Unassigned += OnUnassigned;

            Assignment.Unassign(employee);

            Assert.False(Assignment.IsAssigned(employee));
            Assert.False(Assignment.IsAssigned(recipe));
            Assert.True(UnassignedFlag);
        }

        [UnityTest]
        public IEnumerator ShouldNotUnassignExternal()
        {
            var resources = new Dictionary<Item, int>();
            var product = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var externalRecipe = A.Recipe.WithResources(resources).WithProduct(product).WithExternality(true)
                .WithTime(1)
                .Build();

            yield return CompanyBuilder.WithRecipe(externalRecipe).Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var employee = Company.Employees.ElementAt(0);
            Assignment.Assign(employee, externalRecipe);

            Assert.AreEqual(externalRecipe, Assignment.GetRecipe(employee));
            Assert.True(Assignment.IsAssigned(employee));

            Assignment.Unassign(employee);

            Assert.AreEqual(externalRecipe, Assignment.GetRecipe(employee));
            Assert.True(Assignment.IsAssigned(employee));
        }

        [UnityTest]
        public IEnumerator ShouldUnassignAll()
        {
            var resources = new Dictionary<Item, int>();
            var product = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var externalRecipe = A.Recipe.WithResources(resources).WithProduct(product).WithExternality(true)
                .WithTime(1)
                .Build();

            yield return CompanyBuilder.WithEmployees(3).WithRecipe(externalRecipe).Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var employee1 = Company.Employees.ElementAt(0);
            var employee2 = Company.Employees.ElementAt(1);
            var employee3 = Company.Employees.ElementAt(2);
            var entity2 = employee2.GetComponent<EntityComponent>();
            var internalRecipe = ProductionRegistry.Recipes.ElementAt(0);

            EntityRegistry.Unregister(entity2);

            Assignment.AssignAll(internalRecipe);

            Assert.True(Assignment.IsAssigned(employee1));
            Assert.False(Assignment.IsAssigned(employee2));
            Assert.True(Assignment.IsAssigned(employee3));

            Assignment.UnassignAll();

            Assert.False(Assignment.IsAssigned(employee1));
            Assert.False(Assignment.IsAssigned(employee2));
            Assert.False(Assignment.IsAssigned(employee3));
        }

        [UnityTest]
        public IEnumerator ShouldNotAssignInvalidEmployee()
        {
            yield return CompanyBuilder.Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var employee = Company.Employees.ElementAt(0);
            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            Assignment.Assign(null, recipe);
            Assignment.Assign(employee, null);
            Assignment.Assign(null, null);

            Assert.False(AssignedFlag);
        }

        [UnityTest]
        public IEnumerator ShouldNotUnassignInvalidEmployee()
        {
            yield return CompanyBuilder.Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var otherCompanyBuilder = A.Company.WithOwner(Owner).WithEmployees(1);
            yield return otherCompanyBuilder.Build();
            var otherCompanyObject = otherCompanyBuilder.CompanyObject;
            var otherCompany = otherCompanyObject.GetComponent<CompanyComponent>();
            var otherEmployee = otherCompany.Employees.ElementAt(0);

            Assignment.Unassign(otherEmployee);
            Assignment.Unassign(null);

            Assert.False(UnassignedFlag);
        }

        [UnityTest]
        public IEnumerator ShouldGetAssignedEmployeesAndRecipes()
        {
            var resources1 = new Dictionary<Item, int>();
            var product1 = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var recipe1 = A.Recipe
                .WithName("recipe1")
                .WithResources(resources1)
                .WithProduct(product1)
                .WithTime(1)
                .Build();

            var resources2 = new Dictionary<Item, int>();
            var product2 = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var recipe2 = A.Recipe
                .WithName("recipe2")
                .WithResources(resources2)
                .WithProduct(product2)
                .WithTime(1)
                .Build();

            var recipes = new HashSet<Recipe> { recipe1, recipe2 };

            yield return CompanyBuilder.WithEmployees(4).WithRecipes(recipes).Build();
            CompanyObject = CompanyBuilder.CompanyObject;

            var recipe0 = ProductionRegistry.Recipes.ElementAt(0);

            var employee0 = Company.Employees.ElementAt(0);
            var employee1 = Company.Employees.ElementAt(1);
            var employee2 = Company.Employees.ElementAt(2);

            Assignment.Assign(employee0, recipe0);
            Assignment.Assign(employee1, recipe1);
            Assignment.Assign(employee2, recipe1);

            var recipeForEmployee0 = Assignment.GetRecipe(employee0);
            var recipeForEmployee1 = Assignment.GetRecipe(employee1);
            var recipeForEmployee2 = Assignment.GetRecipe(employee2);

            Debug.Log($"0 {recipeForEmployee0.name}");
            Debug.Log($"1 {recipeForEmployee1.name}");
            Debug.Log($"2 {recipeForEmployee2.name}");

            Debug.Log(employee0 == employee1);

            foreach (var recipe in ProductionRegistry.Recipes) Debug.Log(recipe.name);

            var assignedEmployees = Assignment.GetAssignedEmployees(recipe1);

            Assert.AreEqual(new List<EmployeeComponent> { employee1, employee2 }, assignedEmployees);

            var assignmentCount = Assignment.AssignmentCount(recipe0);

            Assert.AreEqual(1, assignmentCount);

            var assignedRecipes = Assignment.GetAssignedRecipes;

            Assert.AreEqual(new HashSet<Recipe> { recipe0, recipe1 }, assignedRecipes);
        }

        void OnAssigned(object sender, AssignmentEventArgs e)
        {
            AssignedEmployee = e.Employee;
            AssignedRecipe = e.Recipe;
            AssignedFlag = true;
        }

        void OnUnassigned(object sender, AssignmentEventArgs e) => UnassignedFlag = true;
    }
}