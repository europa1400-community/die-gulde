using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using GuldeLib;
using GuldeLib.Builders;
using GuldeLib.Company;
using GuldeLib.Company.Employees;
using GuldeLib.Economy;
using GuldeLib.Entities;
using GuldeLib.Production;
using GuldeLib.Vehicles;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace GuldePlayTests.Vehicles
{
    public class CartAgentComponentTests
    {
        GameBuilder GameBuilder { get; set; }
        CityBuilder CityBuilder { get; set; }
        CompanyBuilder CompanyBuilder { get; set; }
        PlayerBuilder PlayerBuilder { get; set; }

        Item Resource { get; set; }
        Item Product { get; set; }
        Recipe Recipe { get; set; }

        GameObject CompanyObject => CompanyBuilder.CompanyObject;
        CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();
        ProductionAgentComponent ProductionAgent => CompanyObject.GetComponent<ProductionAgentComponent>();
        EmployeeComponent Employee => Company.Employees.ElementAt(0);
        CartComponent Cart => Company.Carts.ElementAt(0);
        CartAgentComponent CartAgent => Cart.GetComponent<CartAgentComponent>();
        EntityComponent CartEntity => Cart.GetComponent<EntityComponent>();

        ExchangeComponent MarketExchange => Locator.Market.Location.Exchanges.ElementAt(0);

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

            PlayerBuilder = A.Player;
            CompanyBuilder = A.Company
                .WithEmployees(1)
                .WithCarts(1)
                .WithRecipe(Recipe)
                .WithEntryCell(4, 4)
                .WithOwner(PlayerBuilder)
                .WithWagePerHour(10)
                .WithMaster();
            CityBuilder = A.City
                .WithSize(20, 20)
                .WithCompany(CompanyBuilder)
                .WithWorkerHome(-4, -4)
                .WithAutoAdvance(true)
                .WithNormalTimeSpeed(100);
            GameBuilder = A.Game
                .WithCity(CityBuilder)
                .WithTimeScale(20f);

            yield return GameBuilder.Build();
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithCartAgent()
        {
            Assert.True(ProductionAgent);
            Assert.True(CartAgent);

            Assert.AreEqual(Company, CartAgent.Cart.Company);
            Assert.False(CartAgent.HasPurchaseOrders);

            Assert.AreEqual(CartAgentComponent.CartState.Idle, CartAgent.State);

            yield break;
        }

        [UnityTest]
        public IEnumerator ShouldAddAndFulfillOrder()
        {
            CompanyBuilder = CompanyBuilder.WithoutMaster();
            yield return GameBuilder.WithTimeScale(1f).Build();

            Cart.AddComponent<CartAgentComponent>();
            MarketExchange.AddItem(Resource);

            var itemOrder = new ItemOrder(Resource, 1);
            CartAgent.AddPurchaseOrder(itemOrder);

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.Contains(itemOrder, CartAgent.PurchaseOrders);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);
            Assert.AreEqual(1, Cart.Exchange.Inventory.GetSupply(Resource));

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Idle, CartAgent.State);
            Assert.AreEqual(1, Company.Exchange.Inventory.GetSupply(Resource));
        }
        
        [UnityTest]
        public IEnumerator ShouldContinueAfterResupply()
        {
            CompanyBuilder = CompanyBuilder.WithoutMaster();
            yield return GameBuilder.WithTimeScale(1f).Build();

            Cart.AddComponent<CartAgentComponent>();
            MarketExchange.AddItem(Resource);

            var itemOrder = new ItemOrder(Resource, 1);
            CartAgent.AddPurchaseOrder(itemOrder);

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.Contains(itemOrder, CartAgent.PurchaseOrders);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);
            Assert.AreEqual(1, Cart.Exchange.Inventory.GetSupply(Resource));

            itemOrder = new ItemOrder(Resource, 1);
            CartAgent.AddPurchaseOrder(itemOrder);
            
            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.AreEqual(1, Company.Exchange.Inventory.GetSupply(Resource));
        }

        [UnityTest]
        public IEnumerator ShouldWaitForMarketResupply()
        {
            CompanyBuilder = CompanyBuilder.WithoutMaster();
            yield return GameBuilder.WithTimeScale(1f).Build();

            Cart.AddComponent<CartAgentComponent>();

            var itemOrder = new ItemOrder(Resource, 1);
            CartAgent.AddPurchaseOrder(itemOrder);

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.Contains(itemOrder, CartAgent.PurchaseOrders);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.Contains(itemOrder, CartAgent.PurchaseOrders);

            yield return Locator.Time.WaitForWorkingHourTicked;

            MarketExchange.AddItem(Resource);
            
            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);
            Assert.AreEqual(1, Cart.Exchange.Inventory.GetSupply(Resource));

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Idle, CartAgent.State);
            Assert.AreEqual(1, Company.Exchange.Inventory.GetSupply(Resource));
        }

        [UnityTest]
        public IEnumerator ShouldIgnoreUnrelatedResupply()
        {
            CompanyBuilder = CompanyBuilder.WithoutMaster();
            yield return GameBuilder.WithTimeScale(1f).Build();

            Cart.AddComponent<CartAgentComponent>();

            var itemOrder = new ItemOrder(Resource, 1);
            CartAgent.AddPurchaseOrder(itemOrder);

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.Contains(itemOrder, CartAgent.PurchaseOrders);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.Contains(itemOrder, CartAgent.PurchaseOrders);

            yield return Locator.Time.WaitForWorkingHourTicked;

            var unrelatedItem = An.Item
                .WithName("unrelatedItem")
                .WithItemType(ItemType.Product)
                .WithMeanPrice(100f)
                .WithMeanSupply(10)
                .WithMinPrice(50f)
                .Build();
            MarketExchange.AddItem(unrelatedItem);

            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.AreEqual(0, Cart.Exchange.Inventory.GetSupply(Resource));
            Assert.AreEqual(0, Cart.Exchange.Inventory.GetSupply(unrelatedItem));
        }

        [UnityTest]
        public IEnumerator ShouldWaitForRecipeFinished()
        {
            var otherResource = An.Item
                .WithName("otherResource")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(100f)
                .WithMinPrice(50f)
                .WithMeanSupply(10)
                .Build();
            var otherProduct = An.Item
                .WithName("otherProduct")
                .WithItemType(ItemType.Product)
                .WithMeanPrice(100f)
                .WithMinPrice(50f)
                .WithMeanSupply(10)
                .Build();
            var otherRecipe = A.Recipe
                .WithName("otherRecipe")
                .WithExternality(false)
                .WithResource(otherResource, 1)
                .WithProduct(otherProduct)
                .WithTime(10)
                .Build();

            CompanyBuilder = CompanyBuilder
                .WithSlots(1, 2)
                .WithRecipe(otherRecipe)
                .WithoutMaster();
            CityBuilder = CityBuilder.WithNormalTimeSpeed(10);
            yield return GameBuilder.WithTimeScale(1f).Build();

            Cart.AddComponent<CartAgentComponent>();

            Company.Exchange.SetLogLevel(LogType.Log);
            Cart.Exchange.SetLogLevel(LogType.Log);

            Company.Production.SetLogLevel(LogType.Log);
            Company.Production.Registry.SetLogLevel(LogType.Log);

            CartAgent.SetLogLevel(LogType.Log);
            Locator.Time.SetLogLevel(LogType.Log);

            Company.Exchange.AddItem(otherResource);
            MarketExchange.AddItem(Resource);

            yield return Locator.Time.WaitForMorning;

            var itemOrder = new ItemOrder(Resource, 1);
            CartAgent.AddPurchaseOrder(itemOrder);

            Assert.True(Company.Production.HasResources(otherRecipe));
            Assert.True(Company.Production.HasProductSlots(otherRecipe));

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.Contains(itemOrder, CartAgent.PurchaseOrders);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.True(Company.Production.HasResources(otherRecipe));
            Assert.True(Company.Production.HasProductSlots(otherRecipe));

            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);
            Assert.AreEqual(1, Cart.Exchange.Inventory.GetSupply(Resource));

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);
            Assert.AreEqual(1, Cart.Exchange.Inventory.GetSupply(Resource));

            Assert.True(Company.Production.HasResources(otherRecipe));
            Assert.True(Company.Production.HasProductSlots(otherRecipe));
            Assert.True(Company.Production.CanProduce(otherRecipe));
            Assert.True(Locator.Time.IsWorkingHour);

            Company.Assignment.Assign(Employee, otherRecipe);

            Assert.True(Company.Assignment.IsAssigned(Employee));
            Assert.True(Company.Assignment.IsAssigned(otherRecipe));
            Assert.True(Company.Production.Registry.IsProducing(otherRecipe));

            yield return Company.Production.Registry.WaitForRecipeFinished(otherRecipe);

            Assert.AreEqual(CartAgentComponent.CartState.Idle, CartAgent.State);
            Assert.AreEqual(1, Company.Exchange.Inventory.GetSupply(Resource));
        }

        [UnityTest]
        public IEnumerator ShouldContinueAfterRecipeFinished()
        {
            var otherResource = An.Item
                .WithName("otherResource")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(100f)
                .WithMinPrice(50f)
                .WithMeanSupply(10)
                .Build();
            var otherProduct = An.Item
                .WithName("otherProduct")
                .WithItemType(ItemType.Product)
                .WithMeanPrice(100f)
                .WithMinPrice(50f)
                .WithMeanSupply(10)
                .Build();
            var otherRecipe = A.Recipe
                .WithName("otherRecipe")
                .WithExternality(false)
                .WithResource(otherResource, 1)
                .WithProduct(otherProduct)
                .WithTime(10)
                .Build();

            CompanyBuilder = CompanyBuilder
                .WithSlots(1, 2)
                .WithRecipe(otherRecipe)
                .WithoutMaster();
            CityBuilder = CityBuilder.WithNormalTimeSpeed(10);
            yield return GameBuilder.WithTimeScale(1f).Build();

            Cart.AddComponent<CartAgentComponent>();

            Company.Exchange.SetLogLevel(LogType.Log);
            Company.Exchange.Inventory.SetLogLevel(LogType.Log);
            Cart.Exchange.Inventory.SetLogLevel(LogType.Log);

            Company.Production.SetLogLevel(LogType.Log);
            Company.Production.Registry.SetLogLevel(LogType.Log);

            CartAgent.SetLogLevel(LogType.Log);
            Locator.Time.SetLogLevel(LogType.Log);

            Company.Exchange.AddItem(otherResource);
            MarketExchange.AddItem(Resource);

            yield return Locator.Time.WaitForMorning;

            var itemOrder = new ItemOrder(Resource, 1);
            CartAgent.AddPurchaseOrder(itemOrder);

            Assert.True(Company.Production.HasResources(otherRecipe));
            Assert.True(Company.Production.HasProductSlots(otherRecipe));

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.Contains(itemOrder, CartAgent.PurchaseOrders);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.True(Company.Production.HasResources(otherRecipe));
            Assert.True(Company.Production.HasProductSlots(otherRecipe));

            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);
            Assert.AreEqual(1, Cart.Exchange.Inventory.GetSupply(Resource));

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);
            Assert.AreEqual(1, Cart.Exchange.Inventory.GetSupply(Resource));

            Assert.True(Company.Production.HasResources(otherRecipe));
            Assert.True(Company.Production.HasProductSlots(otherRecipe));
            Assert.True(Company.Production.CanProduce(otherRecipe));
            Assert.True(Locator.Time.IsWorkingHour);

            Company.Assignment.Assign(Employee, otherRecipe);

            Assert.True(Company.Assignment.IsAssigned(Employee));
            Assert.True(Company.Assignment.IsAssigned(otherRecipe));
            Assert.True(Company.Production.Registry.IsProducing(otherRecipe));

            itemOrder = new ItemOrder(Resource, 1);
            CartAgent.AddPurchaseOrder(itemOrder);
            
            yield return Company.Production.Registry.WaitForRecipeFinished(otherRecipe);

            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.AreEqual(1, Company.Exchange.Inventory.GetSupply(Resource));
        }
    }
}