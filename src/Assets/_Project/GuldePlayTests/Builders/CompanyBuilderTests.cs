using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Builders;
using GuldeLib.Cities;
using GuldeLib.Company;
using GuldeLib.Economy;
using GuldeLib.Production;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace GuldePlayTests.Builders
{
    public class CompanyBuilderTests
    {
        CityBuilder CityBuilder { get; set; }
        CompanyBuilder CompanyBuilder { get; set; }
        PlayerBuilder PlayerBuilder { get; set; }
        GameObject CityObject => CityBuilder.CityObject;
        GameObject CompanyObject => CompanyBuilder.CompanyObject;
        GameObject PlayerObject => PlayerBuilder.PlayerObject;
        CityComponent City => CityObject.GetComponent<CityComponent>();
        CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();
        WealthComponent Owner => PlayerObject.GetComponent<WealthComponent>();
        MasterComponent Master => CompanyObject.GetComponent<MasterComponent>();

        [UnitySetUp]
        public IEnumerator Setup()
        {
            CompanyBuilder = A.Company;
            CityBuilder = A.City
                .WithSize(20, 20)
                .WithCompany(CompanyBuilder);
            PlayerBuilder = A.Player;
            yield return PlayerBuilder.Build();
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            CityBuilder = null;
            CompanyBuilder = null;
            PlayerBuilder = null;

            LogAssert.ignoreFailingMessages = false;
        }

        [UnityTest]
        public IEnumerator ShouldNotBuildCompanyWithoutMap()
        {
            LogAssert.ignoreFailingMessages = true;

            yield return CompanyBuilder.Build();

            Assert.IsNull(CompanyObject);
        }

        [UnityTest]
        public IEnumerator ShouldNotBuildCompanyOutOfBounds()
        {
            LogAssert.ignoreFailingMessages = true;

            CompanyBuilder.WithEntryCell(100, 100);

            yield return CityBuilder.Build();

            Assert.IsNull(CompanyObject);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompany()
        {
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(City.Map.transform, CompanyObject.transform.parent);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithParent()
        {
            var parent = new GameObject();
            CompanyBuilder.WithParent(parent);
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(parent.transform, CompanyObject.transform.parent);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithoutInvalidParent()
        {
            CompanyBuilder.WithParent(null);
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(City.Map.transform, CompanyObject.transform.parent);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithEntryCell()
        {
            CompanyBuilder.WithEntryCell(1, 1);
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(new Vector3Int(1, 1, 0), Company.Location.EntryCell);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithOwner()
        {
            CompanyBuilder.WithOwner(Owner);
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(Owner, Company.Owner);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithSlots()
        {
            CompanyBuilder.WithSlots(3, 2);
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(3, Company.Production.ResourceInventory.Slots);
            Assert.AreEqual(2, Company.Production.ProductInventory.Slots);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompoanyWithWage()
        {
            CompanyBuilder.WithWagePerHour(100);
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(100, Company.WagePerHour);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithEmployees()
        {
            CompanyBuilder.WithEmployees(2);
            yield return CityBuilder.WithWorkerHomes(1).Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(2, Company.Employees.Count);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithCarts()
        {
            CompanyBuilder.WithCarts(3);
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(3, Company.Carts.Count);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithRecipe()
        {
            var resource = An.Item
                .WithName("resource")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(10)
                .WithMinPrice(1)
                .WithMeanSupply(10)
                .Build();
            var product = An.Item
                .WithName("product")
                .WithItemType(ItemType.Product)
                .WithMeanPrice(10)
                .WithMinPrice(1)
                .WithMeanSupply(10)
                .Build();
            var resources = new Dictionary<Item, int>() { {resource, 1} };
            var recipe = A.Recipe
                .WithName("Recipe")
                .WithResources(resources)
                .WithProduct(product)
                .WithExternality(false)
                .WithTime(1)
                .Build();

            CompanyBuilder.WithRecipe(recipe);
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(1, Company.Production.Registry.Recipes.Count);
            Assert.AreEqual(recipe, Company.Production.Registry.Recipes.ElementAt(0));
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithRecipes()
        {
            var resource = An.Item
                .WithName("resource")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(10)
                .WithMinPrice(1)
                .WithMeanSupply(10)
                .Build();
            var product = An.Item
                .WithName("product")
                .WithItemType(ItemType.Product)
                .WithMeanPrice(10)
                .WithMinPrice(1)
                .WithMeanSupply(10)
                .Build();
            var resources1 = new Dictionary<Item, int>() {{resource, 1}};
            var resources2 = new Dictionary<Item, int>() {{resource, 2}};
            var recipe1 = A.Recipe
                .WithName("Recipe")
                .WithResources(resources1)
                .WithProduct(product)
                .WithExternality(false)
                .WithTime(1)
                .Build();
            var recipe2 = A.Recipe
                .WithName("Recipe")
                .WithResources(resources2)
                .WithProduct(product)
                .WithExternality(false)
                .WithTime(1)
                .Build();
            var recipes = new HashSet<Recipe>() {recipe1, recipe2};

            CompanyBuilder.WithRecipes(recipes);
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(2, Company.Production.Registry.Recipes.Count);
            Assert.AreEqual(recipe1, Company.Production.Registry.Recipes.ElementAt(0));
            Assert.AreEqual(recipe2, Company.Production.Registry.Recipes.ElementAt(1));
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithMaster()
        {
            CompanyBuilder = CompanyBuilder.WithMaster(0.25f, 0.5f, 0.75f);
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(0.25f, Master.Riskiness);
            Assert.AreEqual(0.5f, Master.Investivity);
            Assert.AreEqual(0.75f, Master.Autonomy);
        }
    }
}