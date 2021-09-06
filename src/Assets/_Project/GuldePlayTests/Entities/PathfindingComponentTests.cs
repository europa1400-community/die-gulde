using System.Collections;
using GuldeLib.Builders;
using GuldeLib.Cities;
using GuldeLib.Entities;
using GuldeLib.Entities.Pathfinding;
using GuldePlayTests.Builders;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Entities
{
    public class PathfindingComponentTests
    {
        GameBuilder GameBuilder { get; set; }
        CityBuilder CityBuilder { get; set; }
        EntityBuilder EntityBuilder { get; set; }

        GameObject CityObject => CityBuilder.CityObject;
        CityComponent City => CityObject.GetComponent<CityComponent>();
        GameObject EntityObject { get; set; }
        EntityComponent Entity => EntityObject.GetComponent<EntityComponent>();
        PathfindingComponent Pathfinding => EntityObject.GetComponent<PathfindingComponent>();

        public bool DestinationReachedFlag { get; set; }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            CityBuilder = A.City.WithSize(20, 20);
            GameBuilder = A.Game.WithCity(CityBuilder);

            yield return GameBuilder.Build();

            EntityBuilder = A.Entity.WithName("entity").WithMap(City.Map).WithSpeed(2f);
            EntityObject = EntityBuilder.Build();
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            DestinationReachedFlag = false;
        }

        [UnityTest]
        public IEnumerator ShouldFindPath()
        {
            Pathfinding.DestinationReached += OnDestinationReached;
            Pathfinding.SetDestination(new Vector3Int(2, -2, 0));

            yield return Pathfinding.WaitForDestinationReached;

            Assert.True(DestinationReachedFlag);
        }

        [UnityTest]
        public IEnumerator ShouldFindPathFast()
        {
            EntityObject = EntityBuilder.WithSpeed(100f).Build();

            Pathfinding.DestinationReached += OnDestinationReached;
            Pathfinding.SetDestination(new Vector3Int(2, -2, 0));

            yield return Pathfinding.WaitForDestinationReached;

            Assert.True(DestinationReachedFlag);
        }

        [UnityTest]
        public IEnumerator ShouldFindPathWithHighTimeSpeed()
        {
            CityBuilder = CityBuilder.WithNormalTimeSpeed(500);
            yield return GameBuilder.Build();

            EntityObject = EntityBuilder.WithMap(City.Map).Build();

            Pathfinding.DestinationReached += OnDestinationReached;
            Pathfinding.SetDestination(new Vector3Int(2, -2, 0));

            yield return Pathfinding.WaitForDestinationReached;

            Assert.True(DestinationReachedFlag);
        }

        [UnityTest]
        public IEnumerator ShouldFindPathFastWithHighTimeSpeed()
        {
            CityBuilder = CityBuilder.WithNormalTimeSpeed(500);
            yield return GameBuilder.Build();

            EntityObject = EntityBuilder.WithMap(City.Map).WithSpeed(200f).Build();

            Pathfinding.DestinationReached += OnDestinationReached;
            Pathfinding.SetDestination(new Vector3Int(2, -2, 0));

            yield return Pathfinding.WaitForDestinationReached;

            Assert.True(DestinationReachedFlag);
        }

        void OnDestinationReached(object sender, CellEventArgs e)
        {
            DestinationReachedFlag = true;
        }
    }
}