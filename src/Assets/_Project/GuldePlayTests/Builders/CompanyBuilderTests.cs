using System;
using System.Collections;
using Gulde.Builders;
using Gulde.Cities;
using Gulde.Company;
using NUnit.Framework;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Logger = Gulde.Logging.Logger;
using Object = UnityEngine.Object;

namespace GuldePlayTests.Builders
{
    public class CompanyBuilderTests
    {
        CityBuilder CityBuilder { get; set; }
        CompanyBuilder CompanyBuilder { get; set; }
        GameObject CityObject => CityBuilder.CityObject;
        GameObject CompanyObject => CompanyBuilder.CompanyObject;
        CityComponent City => CityObject.GetComponent<CityComponent>();
        CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();

        [SetUp]
        public void Setup()
        {
            CompanyBuilder = A.Company;
            CityBuilder = A.City.WithSize(20, 20).WithCompany(CompanyBuilder);
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            CompanyBuilder = null;

            LogAssert.ignoreFailingMessages = false;
        }

        [UnityTest]
        public IEnumerator ShouldNotBuildCompanyWithoutMap()
        {
            LogAssert.ignoreFailingMessages = true;

            yield return CompanyBuilder.Build();

            Assert.IsNull(CompanyObject);
        }

        [UnityTest]
        public IEnumerator ShouldNotBuildCompanyOutOfBounds()
        {
            LogAssert.ignoreFailingMessages = true;

            CompanyBuilder = CompanyBuilder.WithEntryCell(100, 100);

            yield return CityBuilder.Build();

            Assert.IsNull(CompanyObject);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompany()
        {
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(City.Map.transform, CompanyObject.transform.parent);
        }

        [UnityTest]
        public IEnumerator ShouldBuildCompanyWithParent()
        {
            var parent = new GameObject();
            CompanyBuilder = CompanyBuilder.WithParent(parent);
            yield return CityBuilder.Build();

            Assert.NotNull(CompanyObject);
            Assert.NotNull(Company);
            Assert.AreEqual(parent.transform, CompanyObject.transform.parent);
        }
    }
}