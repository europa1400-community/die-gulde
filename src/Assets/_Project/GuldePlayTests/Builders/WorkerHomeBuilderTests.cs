using System.Collections;
using GuldeLib.Builders;
using GuldeLib.Cities;
using GuldeLib.Company.Employees;
using GuldeLib.Entities;
using GuldeLib.Maps;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Builders
{
    public class WorkerHomeBuilderTests
    {
        CityBuilder CityBuilder { get; set; }
        WorkerHomeBuilder WorkerHomeBuilder { get; set; }

        GameObject CityObject => CityBuilder.CityObject;
        GameObject WorkerHomeObject => WorkerHomeBuilder.WorkerHomeObject;

        CityComponent City => CityObject.GetComponent<CityComponent>();
        EntityRegistryComponent EntityRegistry => WorkerHomeObject.GetComponent<EntityRegistryComponent>();
        LocationComponent Location => WorkerHomeObject.GetComponent<LocationComponent>();
        WorkerHomeComponent WorkerHome => WorkerHomeObject.GetComponent<WorkerHomeComponent>();

        [SetUp]
        public void Setup()
        {
            WorkerHomeBuilder = A.WorkerHome;

            CityBuilder = A.City
                .WithSize(20, 20)
                .WithWorkerHome(WorkerHomeBuilder);
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            CityBuilder = null;
            WorkerHomeBuilder = null;

            LogAssert.ignoreFailingMessages = false;
        }

        [UnityTest]
        public IEnumerator ShouldBuildWorkerHome()
        {
            yield return CityBuilder.Build();

            Assert.NotNull(WorkerHomeObject);
            Assert.NotNull(WorkerHome);
            Assert.NotNull(Location);
            Assert.NotNull(EntityRegistry);
            Assert.AreEqual(City.Map.transform, WorkerHomeObject.transform.parent);
        }

        [UnityTest]
        public IEnumerator ShouldNotBuildWorkerHomeOutOfBounds()
        {
            LogAssert.ignoreFailingMessages = true;

            WorkerHomeBuilder = WorkerHomeBuilder.WithEntryCell(100, 100);
            yield return CityBuilder.Build();

            Assert.IsNull(WorkerHomeObject);
        }

        [UnityTest]
        public IEnumerator ShouldBuildWorkerHomeWithEntryCell()
        {
            var entryCell = new Vector3Int(2, -5, 0);
            WorkerHomeBuilder = WorkerHomeBuilder.WithEntryCell(entryCell);
            yield return CityBuilder.Build();

            Assert.NotNull(WorkerHomeObject);
            Assert.NotNull(WorkerHome);
        }

        [UnityTest]
        public IEnumerator ShouldBuildWorkerHomeWithParent()
        {
            LogAssert.ignoreFailingMessages = true;

            var parent = new GameObject();
            WorkerHomeBuilder = WorkerHomeBuilder.WithParent(parent);
            yield return CityBuilder.Build();

            Assert.NotNull(WorkerHomeObject);
            Assert.NotNull(WorkerHome);
            Assert.AreEqual(parent.transform, WorkerHomeObject.transform.parent);
        }
    }
}