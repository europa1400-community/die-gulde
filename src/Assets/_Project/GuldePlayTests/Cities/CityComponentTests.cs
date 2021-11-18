using System.Collections;
using System.Linq;
using GuldeLib;
using GuldeLib.Builders;
using GuldeLib.Cities;
using GuldeLib.Factories;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Cities
{
    public class CityComponentTests
    {
        CityBuilder CityBuilder { get; set; }
        CityFactory CityFactory { get; set; }
        GameObject CityObject { get; set; }
        CityComponent CityComponent => CityObject.GetComponent<CityComponent>();

        [SetUp]
        public void Setup()
        {
            var map = A.Map
                .WithSize(20, 20);
            CityBuilder = A.City
                .WithMap(map);

            CityFactory = new CityFactory();
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
        public IEnumerator ShouldGetNearestHome()
        {
            var location = A.Location
                .WithEntryCell(-3, -3);
            var market = A.Market
                .WithLocation(location);

            var city = CityBuilder
                .WithWorkerHomes()

            yield return City
                .WithWorkerHome(2, 2)
                .WithWorkerHome(1, 1)
                .WithMarket(marketBuilder)
                .Build();

            var closestHome = CityComponent.GetNearestHome(CityComponent.Market.Location);
            var expectedHome = CityComponent.WorkerHomes.ElementAt(1);

            Assert.AreEqual(expectedHome, closestHome);
        }

        [UnityTest]
        public IEnumerator ShouldGetOnlyHome()
        {
            yield return City
                .WithWorkerHome(1, 1)
                .Build();

            var closestHome = CityComponent.GetNearestHome(CityComponent.Market.Location);
            var expectedHome = CityComponent.WorkerHomes.ElementAt(0);

            Assert.AreEqual(expectedHome, closestHome);
        }

        [UnityTest]
        public IEnumerator ShouldNotGetOnlyHome()
        {
            yield return City
                .Build();

            var closestHome = CityComponent.GetNearestHome(CityComponent.Market.Location);

            Assert.IsNull(closestHome);
        }

        [UnityTest]
        public IEnumerator ShouldRegisterInLocator()
        {
            yield return City.Build();

            Assert.AreEqual(CityComponent, Locator.City);
        }

        [UnityTest]
        public IEnumerator ShouldRegisterCompany()
        {
            var companyBuilder = A.Company;
            yield return City
                .WithCompany(companyBuilder)
                .Build();

            Assert.AreEqual(1, CityComponent.Companies.Count);
            Assert.NotNull(CityComponent.Companies.ElementAt(0));
        }

        [UnityTest]
        public IEnumerator ShouldRegisterWorkerHomes()
        {
            yield return City
                .WithWorkerHomes(3)
                .Build();

            Assert.AreEqual(3, CityComponent.WorkerHomes.Count);
            Assert.NotNull(CityComponent.WorkerHomes.ElementAt(0));
            Assert.NotNull(CityComponent.WorkerHomes.ElementAt(1));
            Assert.NotNull(CityComponent.WorkerHomes.ElementAt(2));
        }

        [UnityTest]
        public IEnumerator ShouldRegisterMarket()
        {
            yield return City.Build();

            Assert.NotNull(CityComponent.Market);
        }
    }
}