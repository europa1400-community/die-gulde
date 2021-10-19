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

            var marketBuilder = A.Market
                .WithExchange("market_exchange_1", An.Exchange);

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
                .WithMarket(marketBuilder)
                .WithCompany(CompanyBuilder)
                .WithWorkerHome(-4, -4)
                .WithAutoAdvance(true)
                .WithNormalTimeSpeed(100);
            GameBuilder = A.Game
                .WithCity(CityBuilder)
                .WithTimeScale(1f);

            yield return GameBuilder.Build();
            Cart.AddComponent<CartAgentComponent>();
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
            CompanyBuilder = CompanyBuilder.WithProductionAgent();
            yield return GameBuilder.Build();

            Assert.True(ProductionAgent);
            Assert.True(CartAgent);

            Assert.AreEqual(Company, CartAgent.Cart.Company);
        }

        [UnityTest]
        public IEnumerator ShouldAddAndFulfillOrder()
        {
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
            MarketExchange.Inventory.Register(Resource);

            var itemOrder = new ItemOrder(Resource, 1);

            CartAgent.SetLogLevel(LogType.Log);
            CartAgent.AddPurchaseOrder(itemOrder);

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.Contains(itemOrder, CartAgent.PurchaseOrders);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.WaitingForResupply, CartAgent.State);
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
            MarketExchange.Inventory.Register(Resource);

            var itemOrder = new ItemOrder(Resource, 1);

            CartAgent.SetLogLevel(LogType.Log);
            CartAgent.AddPurchaseOrder(itemOrder);

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.Contains(itemOrder, CartAgent.PurchaseOrders);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.True(CartAgent.HasPurchaseOrders);
            Assert.AreEqual(CartAgentComponent.CartState.WaitingForResupply, CartAgent.State);
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

            Assert.AreEqual(CartAgentComponent.CartState.WaitingForResupply, CartAgent.State);
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
                .WithRecipe(otherRecipe);
            yield return GameBuilder.Build();

            Cart.AddComponent<CartAgentComponent>();

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
                .WithRecipe(otherRecipe);
            yield return GameBuilder.Build();

            Cart.AddComponent<CartAgentComponent>();

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

        [UnityTest]
        public IEnumerator ShouldAddAndFulfillSaleOrder()
        {
            Company.Exchange.AddItem(Product, 10);
            MarketExchange.AddItem(Product, 1);

            var saleOrder = new ItemOrder(Product, 10);

            Assert.AreEqual(CartAgentComponent.CartState.Idle, CartAgent.State);
            CartAgent.AddSaleOrder(saleOrder);

            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.AreEqual(10, Cart.Exchange.Inventory.GetSupply(Product));

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);
            Assert.AreEqual(0, Cart.Exchange.Inventory.GetSupply(Product));
            Assert.False(CartAgent.HasSaleOrders);
        }

        [UnityTest]
        public IEnumerator ShouldAddAndFulfillSaleOrders()
        {
            CompanyBuilder = CompanyBuilder
                .WithoutCarts()
                .WithCarts(1, CartType.Large);
            yield return GameBuilder.Build();

            Cart.AddComponent<CartAgentComponent>();

            Company.Exchange.AddItem(Resource, 10);
            Company.Exchange.AddItem(Product, 10);
            MarketExchange.AddItem(Resource, 1);
            MarketExchange.AddItem(Product, 1);

            var saleOrderResource = new ItemOrder(Resource, 5);
            var saleOrderProduct = new ItemOrder(Product, 10);
            var orders = new List<ItemOrder> {saleOrderResource, saleOrderProduct};

            Assert.AreEqual(CartAgentComponent.CartState.Idle, CartAgent.State);
            CartAgent.SetLogLevel(LogType.Log);
            CartAgent.AddSaleOrders(orders);

            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.AreEqual(5, Cart.Exchange.Inventory.GetSupply(Resource));
            Assert.AreEqual(10, Cart.Exchange.Inventory.GetSupply(Product));

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);
            Assert.AreEqual(0, Cart.Exchange.Inventory.GetSupply(Resource));
            Assert.AreEqual(0, Cart.Exchange.Inventory.GetSupply(Product));
            Assert.False(CartAgent.HasSaleOrders);
        }

        [UnityTest]
        public IEnumerator ShouldRejectUnsellableSaleOrder()
        {
            Company.Exchange.AddItem(Resource, 10);

            var saleOrder = new ItemOrder(Resource, 5);

            LogAssert.ignoreFailingMessages = true;

            CartAgent.AddSaleOrder(saleOrder);

            Assert.AreEqual(CartAgentComponent.CartState.Idle, CartAgent.State);
            Assert.False(CartAgent.HasSaleOrders);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRejectUnsellableSaleOrders()
        {
            CompanyBuilder = CompanyBuilder
                .WithoutCarts()
                .WithCarts(1, CartType.Large);
            yield return GameBuilder.Build();
            Cart.AddComponent<CartAgentComponent>();

            Company.Exchange.AddItem(Resource, 10);
            Company.Exchange.AddItem(Product, 10);
            MarketExchange.AddItem(Product, 1);

            var saleOrderResource = new ItemOrder(Resource, 5);
            var saleOrderProduct = new ItemOrder(Product, 10);
            var saleOrders = new List<ItemOrder>() {saleOrderResource, saleOrderProduct};

            LogAssert.ignoreFailingMessages = true;

            CartAgent.AddSaleOrders(saleOrders);

            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.AreEqual(1, CartAgent.SaleOrders.Count);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldSellAndBuy()
        {
            Company.Exchange.AddItem(Product, 10);
            MarketExchange.AddItem(Resource, 10);
            MarketExchange.AddItem(Product, 1);

            var purchaseOrder = new ItemOrder(Resource, 10);
            var saleOrder = new ItemOrder(Product, 10);

            CartAgent.SetLogLevel(LogType.Log);

            CartAgent.AddOrders(
                new List<ItemOrder>{purchaseOrder},
                    new List<ItemOrder>{saleOrder});

            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);
            Assert.AreEqual(10, Cart.Exchange.Inventory.GetSupply(Product));

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);
            Assert.AreEqual(10, Cart.Exchange.Inventory.GetSupply(Resource));

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Idle, CartAgent.State);
            Assert.False(CartAgent.HasPurchaseOrders);
            Assert.False(CartAgent.HasSaleOrders);
        }

        [UnityTest]
        public IEnumerator ShouldSellWhileWaitingForMarketRestock()
        {
            CompanyBuilder = CompanyBuilder.WithMaster(0.25f, 0f, 0f);
            yield return GameBuilder.Build();
            Cart.AddComponent<CartAgentComponent>();

            Company.Exchange.AddItem(Product, 10);
            MarketExchange.Inventory.Register(Resource);
            MarketExchange.AddItem(Product, 1);

            var purchaseOrder = new ItemOrder(Resource, 10);
            var saleOrder = new ItemOrder(Product, 10);

            CartAgent.AddPurchaseOrder(purchaseOrder);

            yield return Cart.Travel.WaitForDestinationReached;

            CartAgent.AddSaleOrder(saleOrder);

            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);

            yield return Cart.Travel.WaitForDestinationReached;
            yield return Cart.Travel.WaitForDestinationReached;

            Debug.Log(CartAgent.State);
            Assert.AreEqual(CartAgentComponent.CartState.WaitingForResupply, CartAgent.State);
            Assert.False(CartAgent.HasSaleOrders);
        }

        [UnityTest]
        public IEnumerator ShouldNotSellWhileWaitingForMarketRestock()
        {
            CompanyBuilder = CompanyBuilder.WithMaster(0.75f, 0f, 0f);
            yield return GameBuilder.Build();
            Cart.AddComponent<CartAgentComponent>();

            Company.Exchange.AddItem(Product, 10);
            MarketExchange.Inventory.Register(Resource);
            MarketExchange.AddItem(Product, 1);

            var purchaseOrder = new ItemOrder(Resource, 10);
            var saleOrder = new ItemOrder(Product, 10);

            CartAgent.AddPurchaseOrder(purchaseOrder);

            yield return Cart.Travel.WaitForDestinationReached;

            CartAgent.AddSaleOrder(saleOrder);

            Assert.AreEqual(CartAgentComponent.CartState.WaitingForResupply, CartAgent.State);
            Assert.True(CartAgent.HasSaleOrders);
        }

        [UnityTest]
        public IEnumerator ShouldPurchaseWithMultipleTrips()
        {
            var resource2 = An.Item
                .WithName("resource2")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(10)
                .WithMinPrice(1)
                .WithMeanSupply(10)
                .Build();

            MarketExchange.AddItem(Resource, 10);
            MarketExchange.AddItem(resource2, 10);

            var purchaseOrders = new List<ItemOrder>
            {
                new ItemOrder(Resource, 10),
                new ItemOrder(resource2, 10),
            };

            CartAgent.AddPurchaseOrders(purchaseOrders);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Company, CartAgent.State);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.False(CartAgent.HasPurchaseOrders);
        }

        // [UnityTest]
        // public IEnumerator ShouldSellWithMultipleTrips()
        // {
        //
        // }

        [UnityTest]
        public IEnumerator ShouldSellWhenRecipeFinished()
        {
            MarketExchange.Inventory.Register(Product);
            Company.Exchange.AddItem(Resource);

            var saleOrder = new ItemOrder(Product, 1);

            CartAgent.SetLogLevel(LogType.Log);
            CartAgent.AddSaleOrder(saleOrder);

            yield return Employee.WaitForCompanyReached;

            Company.Assignment.Assign(Employee, Recipe);

            yield return Company.Production.Registry.WaitForRecipeFinished(Recipe);

            Assert.AreEqual(CartAgentComponent.CartState.Market, CartAgent.State);

            yield return Cart.Travel.WaitForDestinationReached;

            Assert.False(CartAgent.HasSaleOrders);
        }
    }
}