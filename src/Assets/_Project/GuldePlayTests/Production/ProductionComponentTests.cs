using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using GuldeLib;
using GuldeLib.Builders;
using GuldeLib.Company;
using GuldeLib.Company.Employees;
using GuldeLib.Economy;
using GuldeLib.Inventory;
using GuldeLib.Production;
using MonoLogger.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace GuldePlayTests.Production
{
    public class ProductionComponentTests
    {
        GameBuilder GameBuilder { get; set; }
        CityBuilder CityBuilder { get; set; }
        CompanyBuilder CompanyBuilder { get; set; }
        Item Resource { get; set; }
        Item Product { get; set; }
        Recipe Recipe { get; set; }

        GameObject CompanyObject => CompanyBuilder.CompanyObject;
        CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();

        EmployeeComponent Employee => Company.Employees.ElementAt(0);

        Item AddedItem { get; set; }
        bool ItemAddedFlag { get; set; }
        int AddedSupply { get; set; }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            Resource = An.Item
                .WithName("resource")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(100f)
                .WithMinPrice(50f)
                .WithMeanSupply(10)
                .Build();
            Product = An.Item
                .WithName("product")
                .WithItemType(ItemType.Product)
                .WithMeanPrice(100f)
                .WithMinPrice(50f)
                .WithMeanSupply(10)
                .Build();
            Recipe = A.Recipe
                .WithName("recipe")
                .WithExternality(false)
                .WithResource(Resource, 1)
                .WithProduct(Product)
                .WithTime(10)
                .Build();

            CompanyBuilder = A.Company
                .WithRecipe(Recipe)
                .WithEmployees(1)
                .WithCarts(1);

            CityBuilder = A.City
                .WithSize(20, 20)
                .WithCompany(CompanyBuilder)
                .WithWorkerHomes(1);

            GameBuilder = A.Game
                .WithCity(CityBuilder)
                .WithTimeScale(10f);

            yield return GameBuilder.Build();
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            FinishedRecipe = null;
            RecipeFinishedFlag = false;
            AddedItem = null;
            AddedSupply = 0;
            ItemAddedFlag = false;
        }

        [UnityTest]
        public IEnumerator ShouldProduce()
        {
            Company.Production.ProductInventory.Added += OnItemAdded;

            yield return Employee.WaitForCompanyReached;

            Company.Assignment.Assign(Employee, Recipe);
            Company.Production.ResourceInventory.AddResources(Recipe);

            yield return Company.Production.Registry.WaitForRecipeFinished(Recipe);

            Assert.True(ItemAddedFlag);
            Assert.AreEqual(Product, AddedItem);
            Assert.AreEqual(1, AddedSupply);
        }

        void OnItemAdded(object sender, ItemEventArgs e)
        {
            ItemAddedFlag = true;
            AddedItem = e.Item;
            AddedSupply = e.Supply;
        }

        [UnityTest]
        public IEnumerator ShouldNotStartInvalidProduction()
        {
            LogAssert.ignoreFailingMessages = true;

            var startProductionMethod =
                typeof(ProductionComponent).GetMethod("StartProduction",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            startProductionMethod?.Invoke(Company.Production, new object[] {null});

            Assert.False(Company.Production.Registry.IsRegistered(null));
            Assert.False(Company.Production.Registry.IsProducing(null));

            Company.Production.ResourceInventory.AddResources(Recipe);
            Company.Production.ResourceInventory.AddResources(Recipe);

            yield return Employee.WaitForCompanyReached;

            Company.Assignment.Assign(Employee, Recipe);

            Assert.True(Company.Production.Registry.IsProducing(Recipe));

            startProductionMethod?.Invoke(Company.Production, new object[] {Recipe});

            Assert.True(Company.Production.HasResources(Recipe));
            Assert.True(Company.Production.Registry.IsProducing(Recipe));
        }

        [Test]
        public void ShouldNotStopInvalidProduction()
        {
            LogAssert.ignoreFailingMessages = true;

            var stopProductionMethod =
                typeof(ProductionComponent).GetMethod("StopProduction",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            stopProductionMethod?.Invoke(Company.Production, new object[] {null});

            Assert.False(Company.Production.Registry.IsRegistered(null));
            Assert.False(Company.Production.Registry.IsProducing(null));

            stopProductionMethod?.Invoke(Company.Production, new object[] {Recipe});

            Assert.False(Company.Production.Registry.IsProducing(Recipe));
        }

        [UnityTest]
        public IEnumerator ShouldAddResourcesWhenHalting()
        {
            yield return Employee.WaitForCompanyReached;

            Company.Production.ResourceInventory.AddResources(Recipe);

            Company.Assignment.Assign(Employee, Recipe);

            yield return Locator.Time.WaitForWorkingHourTicked;

            Assert.True(Company.Production.Registry.IsProducing(Recipe));
            Assert.False(Company.Production.HasResources(Recipe));
            Assert.True(Company.Production.Registry.HasProgress(Recipe));

            Company.Assignment.Unassign(Employee);

            Assert.False(Company.Production.Registry.IsProducing(Recipe));
            Assert.True(Company.Production.HasResources(Recipe));
            Assert.False(Company.Production.Registry.HasProgress(Recipe));
        }

        [UnityTest]
        public IEnumerator ShouldNotProduceWithoutResources()
        {
            Company.Production.Registry.RecipeFinished += OnRecipeFinished;

            yield return Employee.WaitForCompanyReached;

            Assert.False(Company.Production.HasResources(Recipe));
            Assert.False(Company.Production.CanProduce(Recipe));

            Company.Assignment.Assign(Employee, Recipe);

            Assert.False(Company.Production.Registry.IsProducing(Recipe));

            yield return Locator.Time.WaitForEvening;

            Assert.False(RecipeFinishedFlag);
            Assert.IsNull(FinishedRecipe);
        }

        [UnityTest]
        public IEnumerator ShouldRemoveResourcesAfterFinishing()
        {
            Company.Production.SetLogLevel(LogType.Log);
            Company.Production.ResourceInventory.SetLogLevel(LogType.Log);

            yield return Employee.WaitForCompanyReached;

            Assert.True(Company.Production.Registry.IsRegistered(Recipe));
            Company.Exchange.AddItem(Resource);

            Assert.True(Company.Production.HasResources(Recipe));
            Assert.True(Company.Production.CanProduce(Recipe));

            Company.Assignment.Assign(Employee, Recipe);

            Assert.True(Company.Production.Registry.IsProducing(Recipe));

            yield return Company.Production.Registry.WaitForRecipeFinished(Recipe);

            Assert.False(Company.Production.Registry.IsProducing(Recipe));
            Assert.False(Company.Production.HasResources(Recipe));
            Assert.False(Company.Production.CanProduce(Recipe));
        }

        [UnityTest]
        public IEnumerator ShouldRestartHaltedProductionOnItemAdded()
        {
            Company.Production.Registry.RecipeFinished += OnRecipeFinished;

            yield return Employee.WaitForCompanyReached;

            Company.Production.ResourceInventory.Add(Resource);
            Company.Assignment.Assign(Employee, Recipe);

            Assert.True(Company.Production.Registry.IsProducing(Recipe));
            Assert.Contains(Recipe, Company.Production.Registry.ActiveRecipes);

            yield return Company.Production.Registry.WaitForRecipeFinished(Recipe);

            Assert.True(RecipeFinishedFlag);
            Assert.AreEqual(Recipe, FinishedRecipe);
            Assert.False(Company.Production.HasResources(Recipe));
            Assert.False(Company.Production.Registry.IsProducing(Recipe));
            Assert.True(Company.Assignment.IsAssigned(Employee));
            Assert.Contains(Recipe, Company.Production.Registry.HaltedRecipes.ToList());

            Company.Production.ResourceInventory.Add(Resource);

            Assert.False(Company.Production.HasResources(Recipe));
            Assert.True(Company.Production.Registry.IsProducing(Recipe));
        }

        [UnityTest]
        public IEnumerator ShouldRestartHaltedProductionOnEmployeeArrived()
        {
            var longRecipe = A.Recipe
                .WithName("longRecipe")
                .WithExternality(false)
                .WithResource(Resource, 1)
                .WithProduct(Product)
                .WithTime(10000000)
                .Build();

            CompanyBuilder = CompanyBuilder.WithRecipe(longRecipe);
            CityBuilder = CityBuilder.WithAutoAdvance(true);

            yield return GameBuilder.Build();

            Company.Production.Registry.RecipeFinished += OnRecipeFinished;

            yield return Employee.WaitForCompanyReached;

            Company.Production.ResourceInventory.Add(Resource);
            Company.Assignment.Assign(Employee, longRecipe);

            Assert.Contains(longRecipe, Company.Production.Registry.ActiveRecipes);
            Assert.True(Company.Production.Registry.IsProducing(longRecipe));

            yield return Locator.Time.WaitForEvening;

            Assert.False(RecipeFinishedFlag);
            Assert.IsNull(FinishedRecipe);
            Assert.False(Company.Production.HasResources(longRecipe));
            Assert.False(Company.Production.Registry.IsProducing(longRecipe));
            Assert.True(Company.Assignment.IsAssigned(Employee));
            Assert.Contains(longRecipe, Company.Production.Registry.HaltedRecipes.ToList());

            Debug.Log($"Waiting");

            yield return Employee.WaitForCompanyReached;
            yield return Locator.Time.WaitForWorkingHourTicked;

            Debug.Log($"Waited");
            Assert.True(Company.Production.Registry.IsProducing(longRecipe));
        }

        [UnityTest]
        public IEnumerator ShouldUnassignFromExternalWhenFinished()
        {
            var externalRecipe = A.Recipe
                .WithName("externalRecipe")
                .WithExternality(true)
                .WithProduct(Product)
                .WithTime(20)
                .Build();

            CompanyBuilder = CompanyBuilder.WithRecipe(externalRecipe);
            CityBuilder = CityBuilder.WithAutoAdvance(true);

            yield return GameBuilder.Build();

            yield return Employee.WaitForCompanyReached;

            Company.Assignment.Assign(Employee, externalRecipe);

            Assert.True(Company.Assignment.IsAssignedExternally(Employee));
            Assert.True(Company.Assignment.IsAssigned(Employee));
            Assert.True(Company.Production.Registry.IsProducing(externalRecipe));
            Assert.True(Company.Production.Registry.HasProgress(externalRecipe));

            yield return Company.Production.Registry.WaitForRecipeFinished(externalRecipe);

            Assert.False(Company.Assignment.IsAssigned(Employee));
            Assert.False(Company.Assignment.IsAssignedExternally(Employee));
        }

        [Test]
        public void ShouldNotStartProductionWhenNotRegistered()
        {
            var otherRecipe = A.Recipe
                .WithName("otherRecipe")
                .WithExternality(false)
                .WithResource(Resource, 1)
                .WithProduct(Product)
                .WithTime(20)
                .Build();

            Company.Production.Registry.StartProductionRoutine(otherRecipe);

            Assert.False(Company.Production.Registry.IsRegistered(otherRecipe));
            Assert.False(Company.Production.Registry.IsProducing(otherRecipe));
        }

        [Test]
        public void ShouldNotStopProductionWhenNotRegistered()
        {
            var otherRecipe = A.Recipe
                .WithName("otherRecipe")
                .WithExternality(false)
                .WithResource(Resource, 1)
                .WithProduct(Product)
                .WithTime(20)
                .Build();

            Company.Production.Registry.StopProductionRoutine(otherRecipe);

            Assert.False(Company.Production.Registry.IsRegistered(otherRecipe));
            Assert.False(Company.Production.Registry.IsProducing(otherRecipe));
        }

        [Test]
        public void ShouldNotResetProductionWhenNotRegistered()
        {
            var otherRecipe = A.Recipe
                .WithName("otherRecipe")
                .WithExternality(false)
                .WithResource(Resource, 1)
                .WithProduct(Product)
                .WithTime(20)
                .Build();

            Company.Production.Registry.ResetProgress(otherRecipe);

            Assert.False(Company.Production.Registry.IsRegistered(otherRecipe));
            Assert.False(Company.Production.Registry.IsProducing(otherRecipe));
            Assert.False(Company.Production.Registry.HasProgress(otherRecipe));
        }

        [Test]
        public void ShouldGetRecipeForProduct()
        {
            var recipe = Company.Production.Registry.GetRecipe(Product);

            Assert.AreEqual(Recipe, recipe);

            var invalidRecipe = Company.Production.Registry.GetRecipe(Resource);

            Assert.IsNull(invalidRecipe);
        }

        void OnRecipeFinished(object sender, ProductionEventArgs e)
        {
            FinishedRecipe = e.Recipe;
            RecipeFinishedFlag = true;
        }

        public Recipe FinishedRecipe { get; set; }

        public bool RecipeFinishedFlag { get; set; }
    }
}