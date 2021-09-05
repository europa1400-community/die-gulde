using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Builders;
using GuldeLib.Company;
using GuldeLib.Company.Employees;
using GuldeLib.Economy;
using GuldeLib.Entities;
using GuldeLib.Inventory;
using GuldeLib.Production;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Production
{
    public class AssignmentComponentTests
    {
        CityBuilder CityBuilder { get; set; }
        CompanyBuilder CompanyBuilder { get; set; }
        PlayerBuilder PlayerBuilder { get; set; }

        GameObject CityObject => CityBuilder.CityObject;
        GameObject CompanyObject => CompanyBuilder.CompanyObject;
        GameObject PlayerObject => PlayerBuilder.PlayerObject;

        Item Resource { get; set; }
        Item Product { get; set; }

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
            Resource = An.Item
                .WithName("Resource")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(10)
                .WithMinPrice(1)
                .WithMeanSupply(10)
                .Build();
            Product = An.Item
                .WithName("Product")
                .WithItemType(ItemType.Product)
                .WithMeanPrice(10)
                .WithMinPrice(1)
                .WithMeanSupply(10)
                .Build();
            var resources = new Dictionary<Item, int> { {Resource, 1 } };
            var recipe = A.Recipe
                .WithName("recipe0")
                .WithResources(resources)
                .WithProduct(Product)
                .WithExternality(false)
                .WithTime(1)
                .Build();
            var recipes = new HashSet<Recipe> { recipe };

            PlayerBuilder = A.Player;
            yield return PlayerBuilder.Build();

            CompanyBuilder = A.Company
                .WithOwner(Owner)
                .WithSlots(5, 3)
                .WithEmployees(1)
                .WithRecipes(recipes);

            CityBuilder = A.City
                .WithSize(20, 20)
                .WithTime(7, 00, 1400)
                .WithNormalTimeSpeed(300)
                .WithCompany(CompanyBuilder)
                .WithWorkerHome(5, 0)
                .WithAutoAdvance(true);
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
            yield return CityBuilder.Build();

            var recipe = ProductionRegistry.Recipes.ElementAt(0);
            var employee = Company.Employees.ElementAt(0);

            yield return employee.WaitForCompanyReached;

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
            var externalRecipe = A.Recipe
                .WithName("Recipe")
                .WithResources(resources)
                .WithProduct(Product)
                .WithExternality(true)
                .WithTime(1)
                .Build();

            CompanyBuilder = CompanyBuilder.WithRecipe(externalRecipe);
            yield return CityBuilder.Build();

            var internalRecipe = ProductionRegistry.Recipes.ElementAt(0);
            var employee = Company.Employees.ElementAt(0);

            yield return employee.WaitForCompanyReached;

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
            var otherCompanyBuilder = A.Company.WithOwner(Owner).WithEmployees(1);
            yield return CityBuilder
                .WithCompany(otherCompanyBuilder)
                .Build();

            var otherCompanyObject = otherCompanyBuilder.CompanyObject;
            var otherCompany = otherCompanyObject.GetComponent<CompanyComponent>();

            var recipe = ProductionRegistry.Recipes.ElementAt(0);
            var otherEmployee = otherCompany.Employees.ElementAt(0);

            yield return otherEmployee.WaitForCompanyReached;

            Assignment.Assign(otherEmployee, recipe);

            Assert.False(Assignment.IsAssigned(otherEmployee));
            Assert.False(Assignment.IsAssigned(recipe));
            Assert.AreEqual(0, Assignment.AssignmentCount());
        }

        [UnityTest]
        public IEnumerator ShouldNotAssignExternalEmployee()
        {
            var resources = new Dictionary<Item, int>();
            var externalRecipe = A.Recipe
                .WithName("Recipe")
                .WithResources(resources)
                .WithProduct(Product)
                .WithExternality(true)
                .WithTime(1)
                .Build();

            CompanyBuilder = CompanyBuilder.WithRecipe(externalRecipe);
            yield return CityBuilder.Build();

            var employee = Company.Employees.ElementAt(0);

            yield return employee.WaitForCompanyReached;

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
            yield return CityBuilder.Build();

            var employee = Company.Employees.ElementAt(0);
            var entity = employee.GetComponent<EntityComponent>();
            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            yield return employee.WaitForCompanyReached;

            EntityRegistry.Unregister(entity);
            Assignment.Assign(employee, recipe);

            Assert.False(Assignment.IsAssigned(recipe));
            Assert.False(Assignment.IsAssigned(employee));
            Assert.IsNull(Assignment.GetRecipe(employee));
        }

        [UnityTest]
        public IEnumerator ShouldAssignAll()
        {
            CompanyBuilder = CompanyBuilder.WithEmployees(3);
            yield return CityBuilder.Build();

            var employee1 = Company.Employees.ElementAt(0);
            var employee2 = Company.Employees.ElementAt(1);
            var employee3 = Company.Employees.ElementAt(2);
            var entity2 = employee2.GetComponent<EntityComponent>();
            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            yield return employee1.WaitForCompanyReached;
            yield return employee2.WaitForCompanyReached;
            yield return employee3.WaitForCompanyReached;

            EntityRegistry.Unregister(entity2);
            Assignment.AssignAll(recipe);

            var assignedEmployees = Assignment.GetAssignedEmployees(recipe);

            Assert.True(Assignment.IsAssigned(employee1));
            Assert.False(Assignment.IsAssigned(employee2));
            Assert.True(Assignment.IsAssigned(employee3));
            Assert.AreEqual(2, Assignment.AssignmentCount(recipe));
            Assert.Contains(employee1, assignedEmployees);
            Assert.Contains(employee3, assignedEmployees);
        }

        [UnityTest]
        public IEnumerator ShouldUnassign()
        {
            yield return CityBuilder.Build();

            var employee = Company.Employees.ElementAt(0);
            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            yield return employee.WaitForCompanyReached;

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
            var externalRecipe = A.Recipe
                .WithName("Recipe")
                .WithResources(resources)
                .WithProduct(Product)
                .WithExternality(true)
                .WithTime(1)
                .Build();

            CompanyBuilder = CompanyBuilder.WithRecipe(externalRecipe);
            yield return CityBuilder.Build();

            var employee = Company.Employees.ElementAt(0);

            yield return employee.WaitForCompanyReached;

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
            var externalRecipe = A.Recipe
                .WithName("Recipe")
                .WithResources(resources)
                .WithProduct(Product)
                .WithExternality(true)
                .WithTime(1)
                .Build();

            CompanyBuilder = CompanyBuilder.WithEmployees(3).WithRecipe(externalRecipe);
            yield return CityBuilder.Build();

            var employee1 = Company.Employees.ElementAt(0);
            var employee2 = Company.Employees.ElementAt(1);
            var employee3 = Company.Employees.ElementAt(2);
            var entity2 = employee2.GetComponent<EntityComponent>();
            var internalRecipe = ProductionRegistry.Recipes.ElementAt(0);

            yield return employee1.WaitForCompanyReached;
            yield return employee2.WaitForCompanyReached;
            yield return employee3.WaitForCompanyReached;

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
            yield return CityBuilder.Build();

            var employee = Company.Employees.ElementAt(0);
            var recipe = ProductionRegistry.Recipes.ElementAt(0);

            yield return employee.WaitForCompanyReached;

            Assignment.Assign(null, recipe);
            Assignment.Assign(employee, null);
            Assignment.Assign(null, null);

            Assert.False(AssignedFlag);
        }

        [UnityTest]
        public IEnumerator ShouldNotUnassignInvalidEmployee()
        {
            var otherCompanyBuilder = A.Company.WithOwner(Owner).WithEmployees(1);
            yield return CityBuilder.WithCompany(otherCompanyBuilder).Build();

            var otherCompanyObject = otherCompanyBuilder.CompanyObject;
            var otherCompany = otherCompanyObject.GetComponent<CompanyComponent>();
            var otherEmployee = otherCompany.Employees.ElementAt(0);

            yield return otherEmployee.WaitForCompanyReached;

            Assignment.Unassign(otherEmployee);
            Assignment.Unassign(null);

            Assert.False(UnassignedFlag);
        }

        [UnityTest]
        public IEnumerator ShouldGetAssignedEmployeesAndRecipes()
        {
            var resources1 = new Dictionary<Item, int>();
            var recipe1 = A.Recipe
                .WithName("recipe1")
                .WithResources(resources1)
                .WithProduct(Product)
                .WithTime(1)
                .Build();

            var resources2 = new Dictionary<Item, int>();
            var recipe2 = A.Recipe
                .WithName("recipe2")
                .WithResources(resources2)
                .WithProduct(Product)
                .WithTime(2)
                .Build();

            var recipes = new HashSet<Recipe> { recipe1, recipe2 };

            CompanyBuilder = CompanyBuilder.WithEmployees(4).WithRecipes(recipes);
            yield return CityBuilder.Build();

            var recipe0 = ProductionRegistry.Recipes.ElementAt(0);

            var employee0 = Company.Employees.ElementAt(0);
            var employee1 = Company.Employees.ElementAt(1);
            var employee2 = Company.Employees.ElementAt(2);
            var employee3 = Company.Employees.ElementAt(3);

            yield return employee0.WaitForCompanyReached;
            yield return employee1.WaitForCompanyReached;
            yield return employee2.WaitForCompanyReached;
            yield return employee3.WaitForCompanyReached;

            Assignment.Assign(employee0, recipe0);
            Assignment.Assign(employee1, recipe1);
            Assignment.Assign(employee2, recipe1);

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