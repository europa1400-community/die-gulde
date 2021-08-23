using System.Collections.Generic;
using System.Linq;
using Gulde.Company;
using Gulde.Company.Employees;
using Gulde.Economy;
using Gulde.Entities;
using Gulde.Inventory;
using Gulde.Production;
using GuldePlayTests.Builders;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace GuldePlayTests.Production
{
    public class AssignmentComponentTests
    {
        CompanyBuilder CompanyBuilder { get; set; }
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

        bool AssignedFlag { get; set; }
        bool UnassignedFlag { get; set; }

        [SetUp]
        public void Setup()
        {
            var resource = An.Item.WithName("Resource").WithItemType(ItemType.Resource).WithMeanPrice(0f)
                .WithMinPrice(0f).WithMeanSupply(0).Build();
            var product = An.Item.WithName("Product").WithItemType(ItemType.Product).WithMeanPrice(0).WithMinPrice(0)
                .WithMeanSupply(0).Build();

            var resources = new Dictionary<Item, int> { { resource, 1 } };
            var recipe = A.Recipe.WithResources(resources).WithProduct(product).WithExternality(false).WithTime(1)
                .Build();
            var recipes = new HashSet<Recipe> { recipe };

            PlayerObject = A.Player.Build();
            CompanyBuilder = A.Company.WithOwner(Owner).WithSlots(5, 3).WithEmployees(1).WithRecipes(recipes);
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(CompanyObject);
            Object.DestroyImmediate(PlayerObject);

            AssignedFlag = false;
            UnassignedFlag = false;
        }

        [Test]
        public void ShouldAssign()
        {
            CompanyObject = CompanyBuilder.Build();

            var recipe = ProductionRegistry.Recipes.ElementAt(0);
            var employee = Company.Employees.ElementAt(0);

            Assignment.Assigned += OnAssigned;

            Assignment.Assign(employee, recipe);

            Assert.True(Assignment.IsAssigned(employee));
            Assert.True(Assignment.IsAssigned(recipe));
            Assert.True(AssignedFlag);
        }

        [Test]
        public void ShouldReassignFromInternal()
        {
            var resources = new Dictionary<Item, int>();
            var product = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var externalRecipe = A.Recipe.WithResources(resources).WithProduct(product).WithExternality(true)
                .WithTime(1)
                .Build();
            CompanyObject = CompanyBuilder.WithRecipe(externalRecipe).Build();
            var internalRecipe = ProductionRegistry.Recipes.ElementAt(0);
            var employee = Company.Employees.ElementAt(0);
            Assignment.Assign(employee, internalRecipe);

            Assert.True(Assignment.IsAssigned(internalRecipe));
            Assert.AreEqual(internalRecipe, Assignment.GetRecipe(employee));

            Assignment.Assign(employee, externalRecipe);

            Assert.True(Assignment.IsAssigned(externalRecipe));
            Assert.AreEqual(externalRecipe, Assignment.GetRecipe(employee));
        }

        [Test]
        public void ShouldNotAssignUnknownEmployee()
        {
            CompanyObject = CompanyBuilder.Build();
            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            var otherCompanyObject = A.Company.WithOwner(Owner).WithEmployees(1).Build();
            var otherCompany = otherCompanyObject.GetComponent<CompanyComponent>();
            var otherEmployee = otherCompany.Employees.ElementAt(0);

            Assignment.Assign(otherEmployee, recipe);

            Assert.False(Assignment.IsAssigned(otherEmployee));
            Assert.False(Assignment.IsAssigned(recipe));
            Assert.AreEqual(0, Assignment.AssignmentCount());
        }

        [Test]
        public void ShouldNotAssignExternalEmployee()
        {
            var resources = new Dictionary<Item, int>();
            var product = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var externalRecipe = A.Recipe.WithResources(resources).WithProduct(product).WithExternality(true).WithTime(1)
                .Build();
            CompanyObject = CompanyBuilder.WithRecipe(externalRecipe).Build();

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

        [Test]
        public void ShouldNotAssignUnavailableEmployee()
        {
            CompanyObject = CompanyBuilder.Build();

            var employee = Company.Employees.ElementAt(0);
            var entity = employee.GetComponent<EntityComponent>();
            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            EntityRegistry.Unregister(entity);
            Assignment.Assign(employee, recipe);

            Assert.False(Assignment.IsAssigned(recipe));
            Assert.False(Assignment.IsAssigned(employee));
            Assert.IsNull(Assignment.GetRecipe(employee));
        }

        [Test]
        public void ShouldAssignAll()
        {
            CompanyObject = CompanyBuilder.WithEmployees(3).Build();
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

        [Test]
        public void ShouldUnassign()
        {
            CompanyObject = CompanyBuilder.Build();
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

        [Test]
        public void ShouldNotUnassignExternal()
        {
            var resources = new Dictionary<Item, int>();
            var product = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var externalRecipe = A.Recipe.WithResources(resources).WithProduct(product).WithExternality(true)
                .WithTime(1)
                .Build();
            CompanyObject = CompanyBuilder.WithRecipe(externalRecipe).Build();

            var employee = Company.Employees.ElementAt(0);
            Assignment.Assign(employee, externalRecipe);

            Assert.AreEqual(externalRecipe, Assignment.GetRecipe(employee));
            Assert.True(Assignment.IsAssigned(employee));

            Assignment.Unassign(employee);

            Assert.AreEqual(externalRecipe, Assignment.GetRecipe(employee));
            Assert.True(Assignment.IsAssigned(employee));
        }

        [Test]
        public void ShouldUnassignAll()
        {
            var resources = new Dictionary<Item, int>();
            var product = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var externalRecipe = A.Recipe.WithResources(resources).WithProduct(product).WithExternality(true)
                .WithTime(1)
                .Build();

            CompanyObject = CompanyBuilder.WithEmployees(3).WithRecipe(externalRecipe).Build();
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

        [Test]
        public void ShouldNotAssignInvalidEmployee()
        {
            CompanyObject = CompanyBuilder.Build();

            var employee = Company.Employees.ElementAt(0);
            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            Assignment.Assign(null, recipe);
            Assignment.Assign(employee, null);
            Assignment.Assign(null, null);

            Assert.False(AssignedFlag);
        }

        [Test]
        public void ShouldNotUnassignInvalidEmployee()
        {
            CompanyObject = CompanyBuilder.Build();

            var otherCompanyObject = A.Company.WithOwner(Owner).WithEmployees(1).Build();
            var otherCompany = otherCompanyObject.GetComponent<CompanyComponent>();
            var otherEmployee = otherCompany.Employees.ElementAt(0);

            Assignment.Unassign(otherEmployee);
            Assignment.Unassign(null);

            Assert.False(UnassignedFlag);
        }

        [Test]
        public void ShouldGetAssignedEmployeesAndRecipes()
        {
            var resources1 = new Dictionary<Item, int>();
            var product1 = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var recipe1 = A.Recipe.WithResources(resources1).WithProduct(product1).WithTime(1).Build();

            var resources2 = new Dictionary<Item, int>();
            var product2 = An.Item.WithName("Product").WithItemType(ItemType.Product).Build();
            var recipe2 = A.Recipe.WithResources(resources2).WithProduct(product2).WithTime(1).Build();

            var recipes = new HashSet<Recipe> { recipe1, recipe2 };

            CompanyObject = CompanyBuilder.WithEmployees(4).WithRecipes(recipes).Build();

            var recipe0 = ProductionRegistry.Recipes.ElementAt(0);
            var employees = Company.Employees.ToList();

            Assignment.Assign(employees[0], recipe0);
            Assignment.Assign(employees[1], recipe1);
            Assignment.Assign(employees[2], recipe1);

            var assignedEmployees = Assignment.GetAssignedEmployees(recipe1);

            Assert.AreEqual(new List<EmployeeComponent> {employees[1], employees[2] }, assignedEmployees);

            var assignmentCount = Assignment.AssignmentCount(recipe0);

            Assert.AreEqual(1, assignmentCount);

            var assignedRecipes = Assignment.GetAssignedRecipes;

            Assert.AreEqual(new HashSet<Recipe> { recipe0, recipe1 }, assignedRecipes);
        }

        void OnAssigned(object sender, AssignmentEventArgs e) => AssignedFlag = true;

        void OnUnassigned(object sender, AssignmentEventArgs e) => UnassignedFlag = true;
    }
}