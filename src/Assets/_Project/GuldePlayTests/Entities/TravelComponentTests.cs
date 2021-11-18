using System;
using System.Collections;
using System.Linq;
using GuldeLib.Builders;
using GuldeLib.Cities;
using GuldeLib.Entities;
using GuldeLib.Maps;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using MonoLogger.Runtime;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace GuldePlayTests.Entities
{
    public class TravelComponentTests
    {
        CityBuilder CityBuilder { get; set; }

        GameObject CityObject => CityBuilder.CityObject;
        CityComponent City => CityObject.GetComponent<CityComponent>();

        bool DestinationChangedFlag { get; set; }
        bool DestinationReachedFlag { get; set; }
        LocationComponent ChangedDestination { get; set; }
        LocationComponent ReachedDestination { get; set; }

        [SetUp]
        public void Setup()
        {
            CityBuilder = A.City
                .WithSize(20, 20)
                .WithCompany(A.Company.WithEntryCell(2, 0).WithEmployees(1))
                .WithWorkerHome(0, 2)
                .WithMarket(A.Market);
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            DestinationChangedFlag = false;
            DestinationReachedFlag = false;
        }

        [UnityTest]
        public IEnumerator ShouldReachLocation()
        {
            yield return CityBuilder.Build();

            var company = City.Companies.ElementAt(0);
            var employee = company.Employees.ElementAt(0);

            employee.Travel.DestinationReached += OnDestinationReached;

            employee.Travel.TravelTo(City.Market.Location);

            yield return employee.Travel.WaitForDestinationReached;

            Assert.True(DestinationReachedFlag);
            Assert.AreEqual(City.Market.Location, ReachedDestination);
        }

        [UnityTest]
        public IEnumerator ShouldReachChangedDestination()
        {
            yield return CityBuilder.Build();

            var company = City.Companies.ElementAt(0);
            var employee = company.Employees.ElementAt(0);

            employee.Travel.DestinationReached += OnDestinationReached;

            employee.Travel.TravelTo(City.Market.Location);

            yield return employee.Travel.Pathfinder.WaitForDestinationReachedPartly(0.5f);

            employee.Travel.TravelTo(company.Location);

            yield return employee.Travel.WaitForDestinationReached;

            Assert.True(DestinationReachedFlag);
            Assert.AreEqual(company.Location, ReachedDestination);
        }

        [UnityTest]
        public IEnumerator ShouldNotTravelToInvalidLocation()
        {
            yield return CityBuilder.Build();

            var company = City.Companies.ElementAt(0);
            var employee = company.Employees.ElementAt(0);

            employee.Travel.DestinationChanged += OnDestinationChanged;

            employee.Travel.TravelTo(null);

            Assert.False(DestinationChangedFlag);
        }

        [UnityTest]
        public IEnumerator ShouldNotReachInvalidLocation()
        {
            yield return CityBuilder.Build();

            var company = City.Companies.ElementAt(0);
            var employee = company.Employees.ElementAt(0);
            employee.Travel.SetLogLevel(LogType.Log);

            employee.Travel.DestinationReached += OnDestinationReached;

            employee.Travel.TravelTo(City.Market.Location);

            Object.DestroyImmediate(City.Market.gameObject);

            yield return employee.Travel.Pathfinder.WaitForDestinationReached;

            Assert.False(DestinationReachedFlag);
            Assert.IsNull(ReachedDestination);
        }

        void OnDestinationChanged(object sender, LocationEventArgs e)
        {
            DestinationChangedFlag = true;
            ChangedDestination = e.Location;
        }

        void OnDestinationReached(object sender, LocationEventArgs e)
        {
            DestinationReachedFlag = true;
            ReachedDestination = e.Location;
        }
    }
}