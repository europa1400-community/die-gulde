using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GuldeLib;
using GuldeLib.Builders;
using GuldeLib.Companies;
using GuldeLib.Economy;
using GuldeLib.Producing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Economy
{
    public class MarketComponentTests
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
        }

        [Test]
        public void ShouldGetAndCacheExchangeIfExists()
        {
            var exchange = Locator.Market.Location.Exchanges.ElementAt(0);

            var result = Locator.Market.GetExchange(Product);

            Assert.IsNull(result);

            exchange.AddItem(Product);

            result = Locator.Market.GetExchange(Product);

            Assert.AreEqual(exchange, result);

            result = Locator.Market.GetExchange(Product);

            Assert.AreEqual(exchange, result);
        }

        [Test]
        public void ShouldGetPrice()
        {
            var exchange = Locator.Market.Location.Exchanges.ElementAt(0);

            Assert.AreEqual(Product.MeanPrice, Locator.Market.GetPrice(Product));

            exchange.AddItem(Product, 5);

            var result = Locator.Market.GetExchange(Product);
            Assert.AreEqual(result.GetPrice(Product), Locator.Market.GetPrice(Product));
        }
    }
}