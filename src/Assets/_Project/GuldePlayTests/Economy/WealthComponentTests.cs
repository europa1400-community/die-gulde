using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using GuldeLib;
using GuldeLib.Builders;
using GuldeLib.Companies;
using GuldeLib.Economy;
using GuldeLib.Producing;
using MonoLogger.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Economy
{
    public class WealthComponentTests
    {
        PlayerBuilder PlayerBuilder { get; set; }
        GameBuilder GameBuilder { get; set; }
        CityBuilder CityBuilder { get; set; }
        CompanyBuilder CompanyBuilder { get; set; }

        GameObject PlayerObject => PlayerBuilder.PlayerObject;
        WealthComponent Wealth => PlayerObject.GetComponent<WealthComponent>();

        GameObject CompanyObject => CompanyBuilder.CompanyObject;
        CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();

        Item Resource { get; set; }
        Item Product { get; set; }
        Recipe Recipe { get; set; }

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
                .WithOwner(PlayerBuilder)
                .WithCarts(1)
                .WithEmployees(1)
                .WithWagePerHour(1f)
                .WithRecipe(Recipe);
            CityBuilder = A.City
                .WithSize(10, 10)
                .WithMarket(marketBuilder)
                .WithCompany(CompanyBuilder)
                .WithWorkerHomes(1)
                .WithNormalTimeSpeed(100);
            GameBuilder = A.Game
                .WithCity(CityBuilder)
                .WithTimeScale(5f);

            yield return GameBuilder.Build();
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            BilledFlag = false;
            BilledExpenses = null;
            BilledRevenues = null;
        }

        [Test]
        public void ShouldRegisterCompany()
        {
            Assert.Contains(Company, Wealth.Companies);
        }

        [UnityTest]
        public IEnumerator ShouldBillWages()
        {
            Wealth.Billed += OnBilled;

            var virtualExpensesDebug = Wealth.GetType().GetProperty("VirtualExpenses",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(Wealth);
            Assert.AreEqual(0f, virtualExpensesDebug);

            yield return Locator.Time.WaitForWorkingHourTicked;

            var virtualExpenses = Wealth.GetType().GetProperty("VirtualExpenses",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(Wealth);
            Assert.AreEqual(1f, virtualExpenses);

            yield return Wealth.WaitForBilled;

            Assert.AreEqual(14f, BilledExpenses[TurnoverType.Wage]); //TODO This should actually be 13
        }

        [UnityTest]
        public IEnumerator ShouldBillSale()
        {
            Wealth.Billed += OnBilled;

            Company.Production.ProductInventory.Add(Product);
            var cart = Company.Carts.ElementAt(0);
            var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);

            Assert.True(Company.Exchange.CanExchangeWith(cart.Exchange));
            Assert.True(cart.Exchange.IsPurchasing || Company.Exchange.Owner == cart.Exchange.Owner);
            Assert.True(Company.Exchange.ProductInventory.HasItemInStock(Product));
            Assert.True(Company.Exchange.CanSellTo(Product, cart.Exchange));

            Company.Exchange.Sell(Product, cart.Exchange);

            cart.Travel.TravelTo(Locator.Market.Location);

            yield return cart.Travel.WaitForDestinationReached;

            Assert.True(cart.Exchange.CanExchangeWith(marketExchange));
            Assert.True(marketExchange.IsPurchasing && marketExchange.Owner != cart.Exchange.Owner);
            Assert.AreEqual(1, cart.Exchange.Inventory.GetSupply(Product));
            Assert.True(cart.Exchange.CanSellTo(Product, marketExchange));

            cart.Exchange.Sell(Product, marketExchange);

            yield return Wealth.WaitForBilled;

            Assert.AreEqual(Product.MaxPrice, BilledRevenues[TurnoverType.Sale]);
        }

        [UnityTest]
        public IEnumerator ShouldBillPurchase()
        {
            Wealth.Billed += OnBilled;
            Wealth.AddMoney(145f);

            var cart = Company.Carts.ElementAt(0);
            var marketExchange = Locator.Market.Location.Exchanges.ElementAt(0);
            marketExchange.Inventory.Add(Product);

            cart.Travel.TravelTo(Locator.Market.Location);

            yield return cart.Travel.WaitForDestinationReached;

            cart.Exchange.Purchase(Product, marketExchange);

            yield return Locator.Time.WaitForYearTicked;

            Assert.AreEqual(145f, BilledExpenses[TurnoverType.Purchase]);
        }

        [UnityTest]
        public IEnumerator ShouldBillHirings()
        {
            Wealth.Billed += OnBilled;

            yield return Company.HireEmployeeAsync();
            yield return Company.HireCartAsync();

            yield return Wealth.WaitForBilled;

            Assert.AreEqual(Company.HiringCost, BilledExpenses[TurnoverType.Hiring]);
            Assert.AreEqual(Company.CartCost, BilledExpenses[TurnoverType.Cart]);
        }

        [Test]
        public void ShouldAddAndRemoveMoney()
        {
            Wealth.AddMoney(100f);
            Assert.AreEqual(100f, Wealth.Money);
            Wealth.RemoveMoney(80f);
            Assert.AreEqual(20f, Wealth.Money);
        }

        [Test]
        public void ShouldNotRegisterInvalidCompany()
        {
            Wealth.RegisterCompany(null);
        }

        void OnBilled(object sender, BillingEventArgs e)
        {
            BilledFlag = true;
            BilledExpenses = e.Expenses;
            BilledRevenues = e.Revenues;
        }

        bool BilledFlag { get; set; }

        Dictionary<TurnoverType, float> BilledExpenses { get; set; }

        Dictionary<TurnoverType, float> BilledRevenues { get; set; }
    }
}