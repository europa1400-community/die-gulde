using System.Collections;
using System.Linq;
using GuldeLib.Builders;
using GuldeLib.Cities;
using GuldeLib.Company.Employees;
using MonoExtensions.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Builders
{
    public class CityBuilderTests
    {
        CityBuilder CityBuilder { get; set; }
        GameObject CityObject => CityBuilder.CityObject;
        CityComponent City => CityObject.GetComponent<CityComponent>();

        [SetUp]
        public void Setup()
        {
            CityBuilder = A.City;
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
        public IEnumerator ShouldBuildCityWithDefaultSize()
        {
            yield return CityBuilder.Build();

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCityWithSize()
        {
            yield return CityBuilder.WithSize(5, 5).Build();

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
            Assert.AreEqual(new Vector2Int(5, 5), City.Map.Size);
        }

        [UnityTest]
        public IEnumerator ShouldNotBuildCityWithInvalidSize()
        {
            LogAssert.ignoreFailingMessages = true;

            yield return CityBuilder.WithSize(-5, 5).Build();

            Assert.IsNull(CityObject);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCityWithoutWorkerHomeOutOfBounds()
        {
            LogAssert.ignoreFailingMessages = true;

            yield return CityBuilder
                .WithSize(8, 8)
                .WithWorkerHome(5, 5)
                .WithWorkerHome(-5, 5)
                .WithWorkerHome(4, 2)
                .Build();

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
            Assert.IsEmpty(City.WorkerHomes);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCityWithWorkerHome()
        {
            yield return CityBuilder
                .WithSize(8, 8)
                .WithWorkerHome(1, 1)
                .WithWorkerHome(-4, 3)
                .Build();

            var workerHome1 = City.WorkerHomes.ElementAt(0);
            var workerHome2 = City.WorkerHomes.ElementAt(1);

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
            Assert.NotNull(workerHome1);
            Assert.NotNull(workerHome2);
            Assert.AreEqual(2, City.WorkerHomes.Count);
            Assert.AreEqual(City.Map.transform, workerHome1.gameObject.transform.parent);
            Assert.AreEqual(City.Map.transform, workerHome2.gameObject.transform.parent);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCityWithWorkerHomesInBounds()
        {
            yield return CityBuilder.WithSize(8, 8).WithWorkerHomes(3).Build();

            var workerHome1 = City.WorkerHomes.ElementAt(0);
            var workerHome2 = City.WorkerHomes.ElementAt(1);
            var workerHome3 = City.WorkerHomes.ElementAt(2);

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
            foreach (var workerHome in City.WorkerHomes)
            {
                Assert.NotNull(workerHome);
                var cell = workerHome.Location.EntryCell;
                Assert.True(cell.IsInBounds(City.Map.Size));
            }
            Assert.AreEqual(3, City.WorkerHomes.Count);
            Assert.AreNotEqual(workerHome1, workerHome2);
            Assert.AreNotEqual(workerHome2, workerHome3);
            Assert.AreNotEqual(workerHome3, workerHome1);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCityWithoutMarketOutOfBounds()
        {
            yield return CityBuilder.WithMarket(10, 10).Build();

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
            Assert.IsNull(City.Market);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCityWithMarket()
        {
            yield return CityBuilder.WithSize(4, 4).WithMarket(0, 0).Build();

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
            Assert.NotNull(City.Market);
            Assert.AreEqual(new Vector3Int(0, 0, 0), City.Market.Location.EntryCell);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCityWithCompany()
        {
            var companyBuilder = A.Company.WithEntryCell(2, 2);
            yield return CityBuilder.WithSize(8, 8).WithCompany(companyBuilder).Build();

            var company = City.Companies.ElementAt(0);

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
            Assert.NotNull(company);
            Assert.AreEqual(1, City.Companies.Count);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCityWithoutRemovedCompany()
        {
            var companyBuilder = A.Company.WithEntryCell(2, 2);
            yield return CityBuilder
                .WithSize(8, 8)
                .WithCompany(companyBuilder)
                .WithoutCompanies()
                .Build();

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
            Assert.IsEmpty(City.Companies);
            Assert.AreEqual(0, City.Companies.Count);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCityWithTime()
        {
            yield return CityBuilder.WithTime(8, 9, 1410).Build();

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
            Assert.AreEqual(8, City.Time.Hour);
            Assert.AreEqual(9, City.Time.Minute);
            Assert.AreEqual(1410, City.Time.Year);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCityWithTimeSpeed()
        {
            yield return CityBuilder.WithTimeSpeed(2).Build();

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
            Assert.AreEqual(2, City.Time.TimeSpeed);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCityWithAutoAdvance()
        {
            yield return CityBuilder.WithAutoAdvance(true).Build();

            Assert.NotNull(CityObject);
            Assert.NotNull(City);
            Assert.True(City.Time.AutoAdvance);
        }
    }
}