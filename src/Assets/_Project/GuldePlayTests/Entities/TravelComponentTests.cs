using System;
using System.Collections;
using System.Linq;
using Gulde.Builders;
using Gulde.Cities;
using Gulde.Entities;
using Gulde.Maps;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace GuldePlayTests.Entities
{
    public class TravelComponentTests
    {
        CityBuilder CityBuilder { get; set; }

        GameObject CityObject => CityBuilder.CityObject;
        CityComponent City => CityObject.GetComponent<CityComponent>();

        bool LocationReachedFlag { get; set; }
        LocationComponent ReachedLocation { get; set; }

        [SetUp]
        public void Setup()
        {
            CityBuilder = A.City
                .WithSize(20, 20)
                .WithCompany(A.Company.WithEntryCell(2, 0).WithEmployees(1))
                .WithWorkerHome(0, 2)
                .WithMarket(0, 0);
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
        public IEnumerator ShouldNotReachInvalidLocation()
        {
            yield return CityBuilder.Build();

            var company = City.Companies.ElementAt(0);
            var employee = company.Employees.ElementAt(0);

            employee.Travel.LocationReached += OnLocationReached;

            employee.Travel.TravelTo(City.Market.Location);

            Object.DestroyImmediate(City.Market.gameObject);

            yield return employee.Travel.Pathfinding.WaitForDestinationReached;

            Assert.False(LocationReachedFlag);
            Assert.IsNull(ReachedLocation);
        }

        void OnLocationReached(object sender, LocationEventArgs e)
        {
            LocationReachedFlag = true;
            ReachedLocation = e.Location;
        }
    }
}