using System;
using System.Collections;
using System.Linq;
using GuldeLib;
using GuldeLib.Builders;
using GuldeLib.Cities;
using GuldeLib.Company;
using GuldeLib.Company.Employees;
using GuldeLib.Production;
using MonoLogger.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace GuldePlayTests.Company.Employees
{
    public class EmployeeComponentTests
    {
        CityBuilder CityBuilder { get; set; }
        CompanyBuilder CompanyBuilder { get; set; }
        Recipe Recipe { get; set; }
        GameObject CityObject => CityBuilder.CityObject;
        GameObject CompanyObject => CompanyBuilder.CompanyObject;
        CityComponent City => CityObject.GetComponent<CityComponent>();
        CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();
        EmployeeComponent Employee1 => Company.Employees.ElementAt(0);
        EmployeeComponent Employee2 => Company.Employees.ElementAt(1);
        EmployeeComponent Employee3 => Company.Employees.ElementAt(2);

        bool CompanyReachedFlag { get; set; }
        bool HomeReachedFlag { get; set; }

        [SetUp]
        public void Setup()
        {
            var resource = An.Item
                .WithName("resource")
                .WithItemType(ItemType.Product)
                .WithMeanPrice(10)
                .WithMinPrice(1)
                .WithMeanSupply(10)
                .Build();
            var product = An.Item
                .WithName("product")
                .WithItemType(ItemType.Product)
                .WithMeanPrice(10)
                .WithMinPrice(1)
                .WithMeanSupply(10)
                .Build();

            Recipe = A.Recipe
                .WithName("recipe")
                .WithResource(resource, 1)
                .WithProduct(product)
                .WithTime(1)
                .Build();

            CompanyBuilder = A.Company
                .WithEmployees(1)
                .WithRecipe(Recipe);

            CityBuilder = A.City
                .WithSize(10, 10)
                .WithCompany(CompanyBuilder)
                .WithWorkerHomes(1)
                .WithNormalTimeSpeed(500);
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            CityBuilder = null;
            CompanyBuilder = null;
        }

        void OnCompanyReached(object sender, EventArgs e)
        {
            CompanyReachedFlag = true;
        }

        void OnHomeReached(object sender, EventArgs e)
        {
            HomeReachedFlag = true;
        }

        [UnityTest]
        public IEnumerator ShouldReachCompanyAndReturnHome()
        {
            yield return CityBuilder.Build();

            Locator.Time.SetLogLevel(LogType.Log);
            Employee1.Travel.Pathfinding.SetLogLevel(LogType.Log);

            Employee1.CompanyReached += OnCompanyReached;
            Employee1.HomeReached += OnHomeReached;
            yield return Employee1.WaitForCompanyReached;

            Assert.IsTrue(CompanyReachedFlag);
            Assert.IsTrue(Employee1.IsAtCompany);

            yield return Employee1.WaitForHomeReached;

            Assert.IsTrue(HomeReachedFlag);
        }
    }
}