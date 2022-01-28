// <<<<<<< HEAD
// // using System.Collections;
// // using System.Collections.Generic;
// // using System.Linq;
// // using System.Reflection;
// // using GuldeLib;
// // using GuldeLib.Builders;
// // using GuldeLib.Cities;
// // using GuldeLib.Companies;
// // using GuldeLib.Companies.Carts;
// // using GuldeLib.Companies.Employees;
// // using GuldeLib.Economy;
// // using GuldeLib.Producing;
// // using MonoLogger.Runtime;
// // using NUnit.Framework;
// // using UnityEditor.PackageManager;
// // using UnityEngine;
// // using UnityEngine.TestTools;
// //
// // namespace GuldePlayTests.Company
// // {
// //     public class ProductionAgentComponentTests
// //     {
// //         GameBuilder GameBuilder { get; set; }
// //         CityBuilder CityBuilder { get; set; }
// //         CompanyBuilder CompanyBuilder { get; set; }
// //
// //         GameObject CityObject { get; set; }
// //         CityComponent City => CityObject.GetComponent<CityComponent>();
// //         GameObject CompanyObject => CompanyBuilder.CompanyObject;
// //         CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();
// //         EmployeeComponent Employee => Company.Employees.ElementAt(0);
// //         CartComponent Cart => Company.Carts.ElementAt(0);
// //         MasterComponent Master => CompanyObject.GetComponent<MasterComponent>();
// //         ProductionComponent Production => CompanyObject.GetComponent<ProductionComponent>();
// //         ProductionAgentComponent ProductionAgent => Company.GetComponent<ProductionAgentComponent>();
// //         AssignmentComponent Assignment => CompanyObject.GetComponent<AssignmentComponent>();
// //
// //         Item Resource1 { get; set; }
// //         Item Resource2 { get; set; }
// //         Item Resource3 { get; set; }
// //
// //         Item Product1 { get; set; }
// //         Item Product2 { get; set; }
// //
// //         Recipe Recipe1 { get; set; }
// //         Recipe Recipe2 { get; set; }
// //
// //         Recipe Recipe3 { get; set; }
// //
// //         [UnitySetUp]
// //         public IEnumerator Setup()
// //         {
// //             Resource1 = An.Item
// //                 .WithName("resource1")
// //                 .WithItemType(ItemType.Resource)
// //                 .WithMeanPrice(100f)
// //                 .WithMeanSupply(10)
// //                 .WithMinPrice(50f)
// //                 .Build();
// //             Resource2 = An.Item
// //                 .WithName("resource2")
// //                 .WithItemType(ItemType.Resource)
// //                 .WithMeanPrice(100f)
// //                 .WithMeanSupply(10)
// //                 .WithMinPrice(50f)
// //                 .Build();
// //             Resource3 = An.Item
// //                 .WithName("resource3")
// //                 .WithItemType(ItemType.Resource)
// //                 .WithMeanPrice(100f)
// //                 .WithMeanSupply(10)
// //                 .WithMinPrice(50f)
// //                 .Build();
// //
// //             Product1 = An.Item
// //                 .WithName("product1")
// //                 .WithItemType(ItemType.Resource)
// //                 .WithMeanPrice(1000f)
// //                 .WithMeanSupply(10)
// //                 .WithMinPrice(50f)
// //                 .Build();
// //             Product2 = An.Item
// //                 .WithName("product2")
// //                 .WithItemType(ItemType.Product)
// //                 .WithMeanPrice(1000f)
// //                 .WithMeanSupply(10)
// //                 .WithMinPrice(50f)
// //                 .Build();
// //
// //             Recipe1 = A.Recipe
// //                 .WithName("recipe1")
// //                 .WithExternality(false)
// //                 .WithResource(Resource1, 1)
// //                 .WithProduct(Product1)
// //                 .WithTime(120)
// //                 .Build();
// //             Recipe2 = A.Recipe
// //                 .WithName("recipe2")
// //                 .WithExternality(false)
// //                 .WithResource(Resource2, 1)
// //                 .WithResource(Resource3, 1)
// //                 .WithProduct(Product2)
// //                 .WithTime(120)
// //                 .Build();
// //             Recipe3 = A.Recipe
// //                 .WithName("recipe3")
// //                 .WithExternality(false)
// //                 .WithResource(Resource1, 1)
// //                 .WithProduct(Resource2)
// //                 .WithTime(10)
// //                 .Build();
// //
// //             CompanyBuilder = A.Company
// //                 .WithCarts(1)
// //                 .WithEmployees(1)
// //                 .WithEntryCell(3, 3)
// //                 .WithMaster()
// //                 .WithProductionAgent()
// //                 .WithWagePerHour(10)
// //                 .WithSlots(3, 2)
// //                 .WithRecipe(Recipe1)
// //                 .WithRecipe(Recipe2)
// //                 .WithRecipe(Recipe3);
// //
// //             var marketExchangeBuilder = An.Exchange
// //                 .WithItem(Resource1, 0)
// //                 .WithItem(Resource2, 0)
// //                 .WithItem(Resource3, 0)
// //                 .WithItem(Product1, 0)
// //                 .WithItem(Product2, 0);
// //
// //             var marketBuilder = A.Market
// //                 .WithExchange("market_exchange_1", marketExchangeBuilder);
// //
// //             CityBuilder = A.City
// //                 .WithCompany(CompanyBuilder)
// //                 .WithSize(20, 20)
// //                 .WithMarket(marketBuilder)
// //                 .WithNormalTimeSpeed(60)
// //                 .WithAutoAdvance(true)
// //                 .WithWorkerHomes(1);
// //
// //             GameBuilder = A.Game
// //                 .WithCity(CityBuilder)
// //                 .WithTimeScale(10f);
// //
// //             yield return GameBuilder.Build();
// //         }
// //
// //         [TearDown]
// //         public void Teardown()
// //         {
// //             foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
// //             {
// //                 Object.DestroyImmediate(gameObject);
// //             }
// //         }
// //
// //         [UnityTest]
// //         public IEnumerator ShouldGetProfitsPerHour()
// //         {
// //             yield return GameBuilder.WithTimeScale(0.01f).Build();
// //
// //             var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);
// //             marketExchange.AddItem(Resource1, Resource1.MeanSupply);
// //             marketExchange.AddItem(Resource2, Resource2.MeanSupply);
// //             marketExchange.AddItem(Resource3, 20);
// //             marketExchange.AddItem(Product1, 20);
// //             marketExchange.AddItem(Product2, Product2.MeanSupply);
// //
// //             var profitsPerHourProperty = ProductionAgent
// //                 .GetType()
// //                 .GetProperty(
// //                     "ProfitsPerHour",
// //                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
// //             var profitsPerHour = (Dictionary<Recipe, float>)profitsPerHourProperty
// //                 ?.GetValue(ProductionAgent);
// //
// //             Assert.NotNull(profitsPerHour);
// //             Assert.AreEqual(3, profitsPerHour.Count);
// //             Assert.Less(profitsPerHour[Recipe1], profitsPerHour[Recipe2]);
// //             Assert.AreEqual(-50f / 120f, profitsPerHour[Recipe1]);
// //             Assert.AreEqual(850f / 130f, profitsPerHour[Recipe2]);
// //
// //             yield break;
// //         }
// //
// //         [UnityTest]
// //         public IEnumerator ShouldGetSpeculativeProfitsPerHour()
// //         {
// //             yield return GameBuilder.WithTimeScale(0.01f).Build();
// //
// //             var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);
// //             marketExchange.AddItem(Resource1, Resource1.MeanSupply);
// //             marketExchange.AddItem(Resource2, Resource2.MeanSupply);
// //             marketExchange.AddItem(Resource3, 20);
// //             marketExchange.AddItem(Product1, 20);
// //             marketExchange.AddItem(Product2, Product2.MeanSupply);
// //
// //             var speculativeProfitsPerHourProperty = ProductionAgent
// //                 .GetType()
// //                 .GetProperty(
// //                     "SpeculativeProfitsPerHour",
// //                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
// //             var speculativeProfitsPerHour = (Dictionary<Recipe, float>) speculativeProfitsPerHourProperty
// //                 ?.GetValue(ProductionAgent);
// //
// //             Assert.NotNull(speculativeProfitsPerHour);
// //             Assert.AreEqual(3, speculativeProfitsPerHour.Count);
// //             Assert.Less(speculativeProfitsPerHour[Recipe2], speculativeProfitsPerHour[Recipe1]);
// //             Assert.AreEqual(900f / 120f, speculativeProfitsPerHour[Recipe1]);
// //             Assert.AreEqual(850f / 130f, speculativeProfitsPerHour[Recipe2]);
// //
// //             yield break;
// //         }
// //
// //         [UnityTest]
// //         public IEnumerator ShouldGetExpectedProfitsPerHour()
// //         {
// //             yield return GameBuilder.WithTimeScale(0.01f).Build();
// //
// //             var timeProperty = Recipe2
// //                 .GetType()
// //                 .GetProperty(
// //                     "Time",
// //                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
// //             timeProperty?.SetValue(Recipe2, 90);
// //
// //             var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);
// //             marketExchange.SetLogLevel(LogType.Log);
// //             marketExchange.AddItem(Resource1, Resource1.MeanSupply);
// //             marketExchange.AddItem(Resource2, Resource2.MeanSupply);
// //             marketExchange.AddItem(Resource3, 20);
// //             marketExchange.AddItem(Product1, 20);
// //             marketExchange.AddItem(Product2, Product2.MeanSupply);
// //
// //             var expectedProfitsPerHourProperty = ProductionAgent
// //                 .GetType()
// //                 .GetProperty(
// //                     "ExpectedProfitsPerHour",
// //                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
// //             var expectedProfitsPerHour = (Dictionary<Recipe, float>) expectedProfitsPerHourProperty
// //                 ?.GetValue(ProductionAgent);
// //
// //             Assert.NotNull(expectedProfitsPerHour);
// //             Assert.AreEqual(3, expectedProfitsPerHour.Count);
// //             Assert.Less(expectedProfitsPerHour[Recipe1], expectedProfitsPerHour[Recipe2]);
// //             Assert.AreEqual(-50f / 120f, expectedProfitsPerHour[Recipe1]);
// //             Assert.AreEqual(850f / 100f, expectedProfitsPerHour[Recipe2]);
// //
// //             var riskinessProperty = Master
// //                 .GetType()
// //                 .GetProperty(
// //                     "Riskiness",
// //                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
// //             riskinessProperty?.SetValue(Master, 0.5f);
// //
// //             expectedProfitsPerHour = (Dictionary<Recipe, float>) expectedProfitsPerHourProperty
// //                 ?.GetValue(ProductionAgent);
// //
// //             Assert.NotNull(expectedProfitsPerHour);
// //             Assert.AreEqual(3, expectedProfitsPerHour.Count);
// //             Assert.Less(expectedProfitsPerHour[Recipe1], expectedProfitsPerHour[Recipe2]);
// //             Assert.AreEqual((900f / 120f - 50f / 120f) / 2f, expectedProfitsPerHour[Recipe1]);
// //             Assert.AreEqual(850f / 100f, expectedProfitsPerHour[Recipe2]);
// //
// //             riskinessProperty?.SetValue(Master, 1f);
// //             timeProperty?.SetValue(Recipe2, 120);
// //
// //             expectedProfitsPerHour = (Dictionary<Recipe, float>) expectedProfitsPerHourProperty
// //                 ?.GetValue(ProductionAgent);
// //
// //             Assert.NotNull(expectedProfitsPerHour);
// //             Assert.AreEqual(3, expectedProfitsPerHour.Count);
// //             Assert.Less(expectedProfitsPerHour[Recipe2], expectedProfitsPerHour[Recipe1]);
// //             Assert.AreEqual(900f / 120f, expectedProfitsPerHour[Recipe1]);
// //             Assert.AreEqual(850f / 130f, expectedProfitsPerHour[Recipe2]);
// //
// //             yield break;
// //         }
// //
// //         [UnityTest]
// //         public IEnumerator ShouldGetBestRecipes()
// //         {
// //             var bestRecipesProperty = ProductionAgent
// //                 .GetType()
// //                 .GetProperty(
// //                     "BestRecipes",
// //                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
// //             var bestRecipes = (List<Recipe>)bestRecipesProperty
// //                 ?.GetValue(ProductionAgent);
// //
// //             Assert.AreEqual(3, bestRecipes?.Count);
// //             Assert.AreEqual(Recipe1, bestRecipes?.ElementAt(0));
// //
// //             Resource1
// //                 .GetType()
// //                 .GetProperty("MeanPrice", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
// //                 ?.SetValue(Product1, 0f);
// //
// //             bestRecipes = (List<Recipe>)bestRecipesProperty?.GetValue(ProductionAgent);
// //
// //             Assert.AreEqual(3, bestRecipes?.Count);
// //             Assert.AreEqual(Recipe2, bestRecipes?.ElementAt(0));
// //
// //             yield break;
// //         }
// //
// //         [UnityTest]
// //         public IEnumerator ShouldContinueBestProductionWhenFinished()
// //         {
// //             Assert.IsNotNull(ProductionAgent);
// //
// //             // ProductionAgent.SetLogLevel(LogType.Log);
// //             Assignment.SetLogLevel(LogType.Log);
// //             Production.SetLogLevel(LogType.Log);
// //             ProductionAgent.SetLogLevel(LogType.Log);
// //
// //             Company.Exchange.AddItem(Resource1);
// //
// //             yield return Employee.WaitForCompanyReached;
// //
// //             Assert.True(Company.Production.Registry.IsProducing(Recipe1));
// //
// //             Company.Exchange.AddItem(Resource2);
// //             Company.Exchange.AddItem(Resource3);
// //
// //             Resource1
// //                 .GetType()
// //                 .GetProperty("MeanPrice", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
// //                 ?.SetValue(Product1, 0f);
// //
// //             yield return Company.Production.Registry.WaitForRecipeFinished(Recipe1);
// //
// //             Assert.True(Company.Production.Registry.IsProducing(Recipe2));
// //             Assert.False(Company.Production.Registry.IsProducing(Recipe1));
// //         }
// //
// //         [UnityTest]
// //         public IEnumerator ShouldPlaceOrders()
// //         {
// //             CityBuilder = CityBuilder.WithNormalTimeSpeed(10);
// //             yield return GameBuilder.WithTimeScale(1f).Build();
// //
// //             yield return new WaitForEndOfFrame();
// //
// //             var cartAgent = Cart.GetComponent<CartAgentComponent>();
// //
// //             Assert.True(cartAgent.HasPurchaseOrders);
// //             Assert.True(cartAgent.State == CartAgentComponent.CartState.Market);
// //         }
// //
// //         [UnityTest]
// //         public IEnumerator ShouldPlaceOrdersWithLargeCart()
// //         {
// //             CompanyBuilder = CompanyBuilder.WithCarts(0).WithCarts(1, CartType.Large);
// //             CityBuilder = CityBuilder.WithNormalTimeSpeed(10);
// //             yield return GameBuilder.WithTimeScale(1f).Build();
// //
// //             var cartAgent = Cart.GetComponent<CartAgentComponent>();
// //             cartAgent.SetLogLevel(LogType.Log);
// //             ProductionAgent.SetLogLevel(LogType.Log);
// //
// //             Locator.Market.Location.Exchanges.ElementAt(0).AddItem(Resource1);
// //
// //             yield return new WaitForEndOfFrame();
// //
// //             Assert.True(cartAgent.HasPurchaseOrders);
// //             Assert.True(cartAgent.State == CartAgentComponent.CartState.Market);
// //
// //             yield return Employee.WaitForCompanyReached;
// //
// //             Assert.True(Company.Assignment.IsAssigned(Recipe1));
// //
// //             yield return Cart.Travel.WaitForDestinationReached;
// //             yield return Cart.Travel.WaitForDestinationReached;
// //
// //             Assert.True(Company.Production.Registry.IsProducing(Recipe1));
// //         }
// //
// //         [UnityTest]
// //         public IEnumerator ShouldProduceResource()
// //         {
// //             CompanyBuilder = CompanyBuilder.WithoutRecipes()
// //                 .WithRecipe(Recipe2)
// //                 .WithRecipe(Recipe3);
// //
// //             yield return GameBuilder.Build();
// //
// //             var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);
// //             marketExchange.AddItem(Resource1, Resource1.MeanSupply);
// //             marketExchange.AddItem(Resource2, Resource2.MeanSupply);
// //             marketExchange.AddItem(Resource3, Resource3.MeanSupply);
// //             marketExchange.AddItem(Product1, Product1.MeanSupply);
// //             marketExchange.AddItem(Product2, Product2.MeanSupply);
// //
// //             ProductionAgent.SetLogLevel(LogType.Log);
// //             Production.SetLogLevel(LogType.Log);
// //             Production.Registry.SetLogLevel(LogType.Log);
// //             Assignment.SetLogLevel(LogType.Log);
// //
// //             yield return Employee.WaitForCompanyReached;
// //
// //             var targetRecipeProperty = ProductionAgent
// //                 .GetType()
// //                 .GetProperty("TargetRecipe", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
// //             var targetRecipe = (Recipe) targetRecipeProperty?.GetValue(ProductionAgent);
// //
// //             Assert.AreEqual(Recipe2, targetRecipe);
// //             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe2).Count);
// //             Assert.AreEqual(1, Assignment.GetAssignedEmployees(Recipe3).Count);
// //
// //             Assert.True(Production.Registry.IsProducing(Recipe3));
// //             yield return Production.Registry.WaitForRecipeFinished(Recipe3);
// //
// //             targetRecipe = (Recipe) targetRecipeProperty?.GetValue(ProductionAgent);
// //
// //             Assert.AreEqual(Recipe2, targetRecipe);
// //             Assert.AreEqual(1, Assignment.GetAssignedEmployees(Recipe2).Count);
// //             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe3).Count);
// //         }
// //
// //         [UnityTest]
// //         public IEnumerator ShouldProduceResources()
// //         {
// //             Recipe1 = A.Recipe
// //                 .WithName("recipe1")
// //                 .WithExternality(false)
// //                 .WithResource(Resource1, 1)
// //                 .WithProduct(Resource2)
// //                 .WithTime(30)
// //                 .Build();
// //             Recipe2 = A.Recipe
// //                 .WithName("recipe2")
// //                 .WithExternality(false)
// //                 .WithResource(Resource1, 1)
// //                 .WithResource(Resource2, 1)
// //                 .WithProduct(Resource3)
// //                 .WithTime(30)
// //                 .Build();
// //             Recipe3 = A.Recipe
// //                 .WithName("recipe3")
// //                 .WithExternality(false)
// //                 .WithResource(Resource3, 2)
// //                 .WithProduct(Product1)
// //                 .WithTime(60)
// //                 .Build();
// //
// //             CompanyBuilder = CompanyBuilder.WithoutRecipes()
// //                 .WithRecipe(Recipe1)
// //                 .WithRecipe(Recipe2)
// //                 .WithRecipe(Recipe3);
// //
// //             yield return GameBuilder.Build();
// //
// //             var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);
// //             marketExchange.AddItem(Resource1, Resource1.MeanSupply);
// //             marketExchange.AddItem(Resource2, Resource2.MeanSupply);
// //             marketExchange.AddItem(Resource3, Resource3.MeanSupply);
// //             marketExchange.AddItem(Product1, Product1.MeanSupply);
// //             marketExchange.AddItem(Product2, Product2.MeanSupply);
// //
// //             ProductionAgent.SetLogLevel(LogType.Log);
// //             Production.SetLogLevel(LogType.Log);
// //             Production.Registry.SetLogLevel(LogType.Log);
// //             Assignment.SetLogLevel(LogType.Log);
// //
// //             yield return Employee.WaitForCompanyReached;
// //
// //             var targetRecipeProperty = ProductionAgent
// //                 .GetType()
// //                 .GetProperty("TargetRecipe", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
// //             var targetRecipe = (Recipe) targetRecipeProperty?.GetValue(ProductionAgent);
// //
// //             Assert.AreEqual(Recipe3, targetRecipe);
// //             Assert.AreEqual(1, Assignment.GetAssignedEmployees(Recipe1).Count);
// //             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe2).Count);
// //             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe3).Count);
// //
// //             Assert.True(Production.Registry.IsProducing(Recipe1));
// //             yield return Production.Registry.WaitForRecipeFinished(Recipe1);
// //             Assert.True(Production.Registry.IsProducing(Recipe1));
// //             yield return Production.Registry.WaitForRecipeFinished(Recipe1);
// //
// //             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe1).Count);
// //             Assert.AreEqual(1, Assignment.GetAssignedEmployees(Recipe2).Count);
// //             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe3).Count);
// //
// //             Assert.True(Production.Registry.IsProducing(Recipe2));
// //             yield return Production.Registry.WaitForRecipeFinished(Recipe2);
// //             Assert.True(Production.Registry.IsProducing(Recipe2));
// //             yield return Production.Registry.WaitForRecipeFinished(Recipe2);
// //
// //             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe1).Count);
// //             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe2).Count);
// //             Assert.AreEqual(1, Assignment.GetAssignedEmployees(Recipe3).Count);
// //         }
// //     }
// // }
// =======
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using GuldeLib;
// using GuldeLib.Builders;
// using GuldeLib.Cities;
// using GuldeLib.Company;
// using GuldeLib.Company.Employees;
// using GuldeLib.Economy;
// using GuldeLib.Production;
// using GuldeLib.Vehicles;
// using MonoLogger.Runtime;
// using NUnit.Framework;
// using UnityEditor.PackageManager;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// namespace GuldePlayTests.Company
// {
//     public class ProductionAgentComponentTests
//     {
//         GameBuilder GameBuilder { get; set; }
//         CityBuilder CityBuilder { get; set; }
//         CompanyBuilder CompanyBuilder { get; set; }
//
//         GameObject CityObject { get; set; }
//         CityComponent City => CityObject.GetComponent<CityComponent>();
//         GameObject CompanyObject => CompanyBuilder.CompanyObject;
//         CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();
//         EmployeeComponent Employee => Company.Employees.ElementAt(0);
//         CartComponent Cart => Company.Carts.ElementAt(0);
//         MasterComponent Master => CompanyObject.GetComponent<MasterComponent>();
//         ProductionComponent Production => CompanyObject.GetComponent<ProductionComponent>();
//         ProductionAgentComponent ProductionAgent => Company.GetComponent<ProductionAgentComponent>();
//         AssignmentComponent Assignment => CompanyObject.GetComponent<AssignmentComponent>();
//
//         Item Resource1 { get; set; }
//         Item Resource2 { get; set; }
//         Item Resource3 { get; set; }
//
//         Item Product1 { get; set; }
//         Item Product2 { get; set; }
//
//         Recipe Recipe1 { get; set; }
//         Recipe Recipe2 { get; set; }
//
//         Recipe Recipe3 { get; set; }
//
//         [UnitySetUp]
//         public IEnumerator Setup()
//         {
//             Resource1 = An.Item
//                 .WithName("resource1")
//                 .WithItemType(ItemType.Resource)
//                 .WithMeanPrice(100f)
//                 .WithMeanSupply(10)
//                 .WithMinPrice(50f)
//                 .Build();
//             Resource2 = An.Item
//                 .WithName("resource2")
//                 .WithItemType(ItemType.Resource)
//                 .WithMeanPrice(100f)
//                 .WithMeanSupply(10)
//                 .WithMinPrice(50f)
//                 .Build();
//             Resource3 = An.Item
//                 .WithName("resource3")
//                 .WithItemType(ItemType.Resource)
//                 .WithMeanPrice(100f)
//                 .WithMeanSupply(10)
//                 .WithMinPrice(50f)
//                 .Build();
//
//             Product1 = An.Item
//                 .WithName("product1")
//                 .WithItemType(ItemType.Resource)
//                 .WithMeanPrice(1000f)
//                 .WithMeanSupply(10)
//                 .WithMinPrice(50f)
//                 .Build();
//             Product2 = An.Item
//                 .WithName("product2")
//                 .WithItemType(ItemType.Product)
//                 .WithMeanPrice(1000f)
//                 .WithMeanSupply(10)
//                 .WithMinPrice(50f)
//                 .Build();
//
//             Recipe1 = A.Recipe
//                 .WithName("recipe1")
//                 .WithExternality(false)
//                 .WithResource(Resource1, 1)
//                 .WithProduct(Product1)
//                 .WithTime(120)
//                 .Build();
//             Recipe2 = A.Recipe
//                 .WithName("recipe2")
//                 .WithExternality(false)
//                 .WithResource(Resource2, 1)
//                 .WithResource(Resource3, 1)
//                 .WithProduct(Product2)
//                 .WithTime(120)
//                 .Build();
//             Recipe3 = A.Recipe
//                 .WithName("recipe3")
//                 .WithExternality(false)
//                 .WithResource(Resource1, 1)
//                 .WithProduct(Resource2)
//                 .WithTime(10)
//                 .Build();
//
//             CompanyBuilder = A.Company
//                 .WithCarts(1)
//                 .WithEmployees(1)
//                 .WithEntryCell(3, 3)
//                 .WithMaster()
//                 .WithProductionAgent()
//                 .WithWagePerHour(10)
//                 .WithSlots(3, 2)
//                 .WithRecipe(Recipe1)
//                 .WithRecipe(Recipe2)
//                 .WithRecipe(Recipe3);
//
//             var marketExchangeBuilder = An.Exchange
//                 .WithItem(Resource1, 0)
//                 .WithItem(Resource2, 0)
//                 .WithItem(Resource3, 0)
//                 .WithItem(Product1, 0)
//                 .WithItem(Product2, 0);
//
//             var marketBuilder = A.Market
//                 .WithExchange("market_exchange_1", marketExchangeBuilder);
//
//             CityBuilder = A.City
//                 .WithCompany(CompanyBuilder)
//                 .WithSize(20, 20)
//                 .WithMarket(marketBuilder)
//                 .WithNormalTimeSpeed(60)
//                 .WithAutoAdvance(true)
//                 .WithWorkerHomes(1);
//
//             GameBuilder = A.Game
//                 .WithCity(CityBuilder)
//                 .WithTimeScale(10f);
//
//             yield return GameBuilder.Build();
//         }
//
//         [TearDown]
//         public void Teardown()
//         {
//             foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
//             {
//                 Object.DestroyImmediate(gameObject);
//             }
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldGetProfitsPerHour()
//         {
//             yield return GameBuilder.WithTimeScale(0.01f).Build();
//
//             var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);
//             marketExchange.AddItem(Resource1, Resource1.MeanSupply);
//             marketExchange.AddItem(Resource2, Resource2.MeanSupply);
//             marketExchange.AddItem(Resource3, 20);
//             marketExchange.AddItem(Product1, 20);
//             marketExchange.AddItem(Product2, Product2.MeanSupply);
//
//             var profitsPerHourProperty = ProductionAgent
//                 .GetType()
//                 .GetProperty(
//                     "ProfitsPerHour",
//                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//             var profitsPerHour = (Dictionary<Recipe, float>)profitsPerHourProperty
//                 ?.GetValue(ProductionAgent);
//
//             Assert.NotNull(profitsPerHour);
//             Assert.AreEqual(3, profitsPerHour.Count);
//             Assert.Less(profitsPerHour[Recipe1], profitsPerHour[Recipe2]);
//             Assert.AreEqual(-50f / 120f, profitsPerHour[Recipe1]);
//             Assert.AreEqual(850f / 130f, profitsPerHour[Recipe2]);
//
//             yield break;
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldGetSpeculativeProfitsPerHour()
//         {
//             yield return GameBuilder.WithTimeScale(0.01f).Build();
//
//             var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);
//             marketExchange.AddItem(Resource1, Resource1.MeanSupply);
//             marketExchange.AddItem(Resource2, Resource2.MeanSupply);
//             marketExchange.AddItem(Resource3, 20);
//             marketExchange.AddItem(Product1, 20);
//             marketExchange.AddItem(Product2, Product2.MeanSupply);
//
//             var speculativeProfitsPerHourProperty = ProductionAgent
//                 .GetType()
//                 .GetProperty(
//                     "SpeculativeProfitsPerHour",
//                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//             var speculativeProfitsPerHour = (Dictionary<Recipe, float>) speculativeProfitsPerHourProperty
//                 ?.GetValue(ProductionAgent);
//
//             Assert.NotNull(speculativeProfitsPerHour);
//             Assert.AreEqual(3, speculativeProfitsPerHour.Count);
//             Assert.Less(speculativeProfitsPerHour[Recipe2], speculativeProfitsPerHour[Recipe1]);
//             Assert.AreEqual(900f / 120f, speculativeProfitsPerHour[Recipe1]);
//             Assert.AreEqual(850f / 130f, speculativeProfitsPerHour[Recipe2]);
//
//             yield break;
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldGetExpectedProfitsPerHour()
//         {
//             yield return GameBuilder.WithTimeScale(0.01f).Build();
//
//             var timeProperty = Recipe2
//                 .GetType()
//                 .GetProperty(
//                     "Time",
//                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//             timeProperty?.SetValue(Recipe2, 90);
//
//             var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);
//             marketExchange.SetLogLevel(LogType.Log);
//             marketExchange.AddItem(Resource1, Resource1.MeanSupply);
//             marketExchange.AddItem(Resource2, Resource2.MeanSupply);
//             marketExchange.AddItem(Resource3, 20);
//             marketExchange.AddItem(Product1, 20);
//             marketExchange.AddItem(Product2, Product2.MeanSupply);
//
//             var expectedProfitsPerHourProperty = ProductionAgent
//                 .GetType()
//                 .GetProperty(
//                     "ExpectedProfitsPerHour",
//                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//             var expectedProfitsPerHour = (Dictionary<Recipe, float>) expectedProfitsPerHourProperty
//                 ?.GetValue(ProductionAgent);
//
//             Assert.NotNull(expectedProfitsPerHour);
//             Assert.AreEqual(3, expectedProfitsPerHour.Count);
//             Assert.Less(expectedProfitsPerHour[Recipe1], expectedProfitsPerHour[Recipe2]);
//             Assert.AreEqual(-50f / 120f, expectedProfitsPerHour[Recipe1]);
//             Assert.AreEqual(850f / 100f, expectedProfitsPerHour[Recipe2]);
//
//             var riskinessProperty = Master
//                 .GetType()
//                 .GetProperty(
//                     "Riskiness",
//                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//             riskinessProperty?.SetValue(Master, 0.5f);
//
//             expectedProfitsPerHour = (Dictionary<Recipe, float>) expectedProfitsPerHourProperty
//                 ?.GetValue(ProductionAgent);
//
//             Assert.NotNull(expectedProfitsPerHour);
//             Assert.AreEqual(3, expectedProfitsPerHour.Count);
//             Assert.Less(expectedProfitsPerHour[Recipe1], expectedProfitsPerHour[Recipe2]);
//             Assert.AreEqual((900f / 120f - 50f / 120f) / 2f, expectedProfitsPerHour[Recipe1]);
//             Assert.AreEqual(850f / 100f, expectedProfitsPerHour[Recipe2]);
//
//             riskinessProperty?.SetValue(Master, 1f);
//             timeProperty?.SetValue(Recipe2, 120);
//
//             expectedProfitsPerHour = (Dictionary<Recipe, float>) expectedProfitsPerHourProperty
//                 ?.GetValue(ProductionAgent);
//
//             Assert.NotNull(expectedProfitsPerHour);
//             Assert.AreEqual(3, expectedProfitsPerHour.Count);
//             Assert.Less(expectedProfitsPerHour[Recipe2], expectedProfitsPerHour[Recipe1]);
//             Assert.AreEqual(900f / 120f, expectedProfitsPerHour[Recipe1]);
//             Assert.AreEqual(850f / 130f, expectedProfitsPerHour[Recipe2]);
//
//             yield break;
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldGetBestRecipes()
//         {
//             var bestRecipesProperty = ProductionAgent
//                 .GetType()
//                 .GetProperty(
//                     "BestRecipes",
//                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//             var bestRecipes = (List<Recipe>)bestRecipesProperty
//                 ?.GetValue(ProductionAgent);
//
//             Assert.AreEqual(3, bestRecipes?.Count);
//             Assert.AreEqual(Recipe1, bestRecipes?.ElementAt(0));
//
//             Resource1
//                 .GetType()
//                 .GetProperty("MeanPrice", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
//                 ?.SetValue(Product1, 0f);
//
//             bestRecipes = (List<Recipe>)bestRecipesProperty?.GetValue(ProductionAgent);
//
//             Assert.AreEqual(3, bestRecipes?.Count);
//             Assert.AreEqual(Recipe2, bestRecipes?.ElementAt(0));
//
//             yield break;
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldContinueBestProductionWhenFinished()
//         {
//             Assert.IsNotNull(ProductionAgent);
//
//             // ProductionAgent.SetLogLevel(LogType.Log);
//             Assignment.SetLogLevel(LogType.Log);
//             Production.SetLogLevel(LogType.Log);
//             ProductionAgent.SetLogLevel(LogType.Log);
//
//             Company.Exchange.AddItem(Resource1);
//
//             yield return Employee.WaitForCompanyReached;
//
//             Assert.True(Company.Production.Registry.IsProducing(Recipe1));
//
//             Company.Exchange.AddItem(Resource2);
//             Company.Exchange.AddItem(Resource3);
//
//             Resource1
//                 .GetType()
//                 .GetProperty("MeanPrice", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
//                 ?.SetValue(Product1, 0f);
//
//             yield return Company.Production.Registry.WaitForRecipeFinished(Recipe1);
//
//             Assert.True(Company.Production.Registry.IsProducing(Recipe2));
//             Assert.False(Company.Production.Registry.IsProducing(Recipe1));
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldPlaceOrders()
//         {
//             CityBuilder = CityBuilder.WithNormalTimeSpeed(10);
//             yield return GameBuilder.WithTimeScale(1f).Build();
//
//             yield return null;
//
//             var cartAgent = Cart.GetComponent<CartAgentComponent>();
//
//             Assert.True(cartAgent.HasPurchaseOrders);
//             Assert.True(cartAgent.State == CartAgentComponent.CartState.Market);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldPlaceOrdersWithLargeCart()
//         {
//             CompanyBuilder = CompanyBuilder.WithCarts(0).WithCarts(1, CartType.Large);
//             CityBuilder = CityBuilder.WithNormalTimeSpeed(10);
//             yield return GameBuilder.WithTimeScale(1f).Build();
//
//             var cartAgent = Cart.GetComponent<CartAgentComponent>();
//             cartAgent.SetLogLevel(LogType.Log);
//             ProductionAgent.SetLogLevel(LogType.Log);
//
//             Locator.Market.Location.Exchanges.ElementAt(0).AddItem(Resource1);
//
//             yield return null;
//
//             Assert.True(cartAgent.HasPurchaseOrders);
//             Assert.True(cartAgent.State == CartAgentComponent.CartState.Market);
//
//             yield return Employee.WaitForCompanyReached;
//
//             Assert.True(Company.Assignment.IsAssigned(Recipe1));
//
//             yield return Cart.Travel.WaitForDestinationReached;
//             yield return Cart.Travel.WaitForDestinationReached;
//
//             Assert.True(Company.Production.Registry.IsProducing(Recipe1));
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldProduceResource()
//         {
//             CompanyBuilder = CompanyBuilder.WithoutRecipes()
//                 .WithRecipe(Recipe2)
//                 .WithRecipe(Recipe3);
//
//             yield return GameBuilder.Build();
//
//             var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);
//             marketExchange.AddItem(Resource1, Resource1.MeanSupply);
//             marketExchange.AddItem(Resource2, Resource2.MeanSupply);
//             marketExchange.AddItem(Resource3, Resource3.MeanSupply);
//             marketExchange.AddItem(Product1, Product1.MeanSupply);
//             marketExchange.AddItem(Product2, Product2.MeanSupply);
//
//             ProductionAgent.SetLogLevel(LogType.Log);
//             Production.SetLogLevel(LogType.Log);
//             Production.Registry.SetLogLevel(LogType.Log);
//             Assignment.SetLogLevel(LogType.Log);
//
//             yield return Employee.WaitForCompanyReached;
//
//             var targetRecipeProperty = ProductionAgent
//                 .GetType()
//                 .GetProperty("TargetRecipe", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//             var targetRecipe = (Recipe) targetRecipeProperty?.GetValue(ProductionAgent);
//
//             Assert.AreEqual(Recipe2, targetRecipe);
//             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe2).Count);
//             Assert.AreEqual(1, Assignment.GetAssignedEmployees(Recipe3).Count);
//
//             Assert.True(Production.Registry.IsProducing(Recipe3));
//             yield return Production.Registry.WaitForRecipeFinished(Recipe3);
//
//             targetRecipe = (Recipe) targetRecipeProperty?.GetValue(ProductionAgent);
//
//             Assert.AreEqual(Recipe2, targetRecipe);
//             Assert.AreEqual(1, Assignment.GetAssignedEmployees(Recipe2).Count);
//             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe3).Count);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldProduceResources()
//         {
//             Recipe1 = A.Recipe
//                 .WithName("recipe1")
//                 .WithExternality(false)
//                 .WithResource(Resource1, 1)
//                 .WithProduct(Resource2)
//                 .WithTime(30)
//                 .Build();
//             Recipe2 = A.Recipe
//                 .WithName("recipe2")
//                 .WithExternality(false)
//                 .WithResource(Resource1, 1)
//                 .WithResource(Resource2, 1)
//                 .WithProduct(Resource3)
//                 .WithTime(30)
//                 .Build();
//             Recipe3 = A.Recipe
//                 .WithName("recipe3")
//                 .WithExternality(false)
//                 .WithResource(Resource3, 2)
//                 .WithProduct(Product1)
//                 .WithTime(60)
//                 .Build();
//
//             CompanyBuilder = CompanyBuilder.WithoutRecipes()
//                 .WithRecipe(Recipe1)
//                 .WithRecipe(Recipe2)
//                 .WithRecipe(Recipe3);
//
//             yield return GameBuilder.Build();
//
//             var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);
//             marketExchange.AddItem(Resource1, Resource1.MeanSupply);
//             marketExchange.AddItem(Resource2, Resource2.MeanSupply);
//             marketExchange.AddItem(Resource3, Resource3.MeanSupply);
//             marketExchange.AddItem(Product1, Product1.MeanSupply);
//             marketExchange.AddItem(Product2, Product2.MeanSupply);
//
//             ProductionAgent.SetLogLevel(LogType.Log);
//             Production.SetLogLevel(LogType.Log);
//             Production.Registry.SetLogLevel(LogType.Log);
//             Assignment.SetLogLevel(LogType.Log);
//
//             yield return Employee.WaitForCompanyReached;
//
//             var targetRecipeProperty = ProductionAgent
//                 .GetType()
//                 .GetProperty("TargetRecipe", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//             var targetRecipe = (Recipe) targetRecipeProperty?.GetValue(ProductionAgent);
//
//             Assert.AreEqual(Recipe3, targetRecipe);
//             Assert.AreEqual(1, Assignment.GetAssignedEmployees(Recipe1).Count);
//             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe2).Count);
//             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe3).Count);
//
//             Assert.True(Production.Registry.IsProducing(Recipe1));
//             yield return Production.Registry.WaitForRecipeFinished(Recipe1);
//             Assert.True(Production.Registry.IsProducing(Recipe1));
//             yield return Production.Registry.WaitForRecipeFinished(Recipe1);
//
//             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe1).Count);
//             Assert.AreEqual(1, Assignment.GetAssignedEmployees(Recipe2).Count);
//             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe3).Count);
//
//             Assert.True(Production.Registry.IsProducing(Recipe2));
//             yield return Production.Registry.WaitForRecipeFinished(Recipe2);
//             Assert.True(Production.Registry.IsProducing(Recipe2));
//             yield return Production.Registry.WaitForRecipeFinished(Recipe2);
//
//             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe1).Count);
//             Assert.AreEqual(0, Assignment.GetAssignedEmployees(Recipe2).Count);
//             Assert.AreEqual(1, Assignment.GetAssignedEmployees(Recipe3).Count);
//         }
//     }
// }
// >>>>>>> master
