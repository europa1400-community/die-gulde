using System.Collections;
using GuldeLib.Builders;
using GuldeLib.Cities;
using GuldeLib.Entities;
using GuldeLib.Pathfinding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Builders
{
    public class EntityBuilderTests
    {
        GameBuilder GameBuilder { get; set; }
        CityBuilder CityBuilder { get; set; }

        GameObject CityObject => CityBuilder.CityObject;
        CityComponent City => CityObject.GetComponent<CityComponent>();

        [UnitySetUp]
        public IEnumerator Setup()
        {
            CityBuilder = A.City.WithSize(10, 10);
            GameBuilder = A.Game.WithCity(CityBuilder);

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
        public void ShouldBuildEntity()
        {
            var entityObject = A.Entity.WithName("entity").WithMap(City.Map).Build();
            var entity = entityObject.GetComponent<EntityComponent>();

            Assert.NotNull(entity);
        }

        [Test]
        public void ShouldBuildEntityWithName()
        {
            var entityObject = A.Entity.WithName("some_entity").WithMap(City.Map).Build();

            Assert.AreEqual("some_entity", entityObject.name);
        }

        [Test]
        public void ShouldBuildEntityWithSpeed()
        {
            var entityObject = A.Entity.WithName("entity").WithMap(City.Map).WithSpeed(21f).Build();
            var pathfinding = entityObject.GetComponent<PathfinderComponent>();

            Assert.AreEqual(21f, pathfinding.Speed);
        }

        [Test]
        public void ShouldNotBuildEntityWithoutName()
        {
            LogAssert.ignoreFailingMessages = true;
            var entityObject = A.Entity.WithMap(City.Map).Build();

            Assert.IsNull(entityObject);
        }

        [Test]
        public void ShouldNotBuildEntityWithoutMap()
        {
            LogAssert.ignoreFailingMessages = true;
            var entityObject = A.Entity.WithName("entity").Build();

            Assert.IsNull(entityObject);
        }
    }
}