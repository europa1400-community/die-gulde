using System.Collections;
using System.Linq;
using GuldeLib;
using GuldeLib.Builders;
using GuldeLib.Cities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Cities
{
    public class CityComponentTests
    {
        CityBuilder CityBuilder { get; set; }
        GameObject CityObject => CityBuilder.CityObject;
        CityComponent City => CityObject.GetComponent<CityComponent>();

        [SetUp]
        public void Setup()
        {
            CityBuilder = A.City
                .WithSize(20, 20);
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            CityBuilder = null;
        }

        [UnityTest]
        public IEnumerator ShouldGetNearestHome()
        {
            var marketBuilder = A.Market
                .WithEntryCell(-3, -3);

            yield return CityBuilder
                .WithWorkerHome(2, 2)
                .WithWorkerHome(1, 1)
                .WithMarket(marketBuilder)
                .Build();

            var closestHome = City.GetNearestHome(City.Market.Location);
            var expectedHome = City.WorkerHomes.ElementAt(1);

            Assert.AreEqual(expectedHome, closestHome);
        }

        [UnityTest]
        public IEnumerator ShouldGetOnlyHome()
        {
            yield return CityBuilder
                .WithWorkerHome(1, 1)
                .Build();

            var closestHome = City.GetNearestHome(City.Market.Location);
            var expectedHome = City.WorkerHomes.ElementAt(0);

            Assert.AreEqual(expectedHome, closestHome);
        }

        [UnityTest]
        public IEnumerator ShouldNotGetOnlyHome()
        {
            yield return CityBuilder
                .Build();

            var closestHome = City.GetNearestHome(City.Market.Location);

            Assert.IsNull(closestHome);
        }

        [UnityTest]
        public IEnumerator ShouldRegisterInLocator()
        {
            yield return CityBuilder.Build();

            Assert.AreEqual(City, Locator.City);
        }

        [UnityTest]
        public IEnumerator ShouldRegisterCompany()
        {
            var companyBuilder = A.Company;
            yield return CityBuilder
                .WithCompany(companyBuilder)
                .Build();

            Assert.AreEqual(1, City.Companies.Count);
            Assert.NotNull(City.Companies.ElementAt(0));
        }

        [UnityTest]
        public IEnumerator ShouldRegisterWorkerHomes()
        {
            yield return CityBuilder
                .WithWorkerHomes(3)
                .Build();

            Assert.AreEqual(3, City.WorkerHomes.Count);
            Assert.NotNull(City.WorkerHomes.ElementAt(0));
            Assert.NotNull(City.WorkerHomes.ElementAt(1));
            Assert.NotNull(City.WorkerHomes.ElementAt(2));
        }

        [UnityTest]
        public IEnumerator ShouldRegisterMarket()
        {
            yield return CityBuilder.Build();

            Assert.NotNull(City.Market);
        }
    }
}