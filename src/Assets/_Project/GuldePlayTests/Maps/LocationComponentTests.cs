using System.Collections;
using GuldeLib.Builders;
using GuldeLib.Maps;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Maps
{
    public class LocationComponentTests
    {
        GameBuilder GameBuilder { get; set; }
        CityBuilder CityBuilder { get; set; }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            CityBuilder = A.City.WithSize(10, 10);
            GameBuilder = A.Game.WithCity(CityBuilder);

            yield return null;
        }

        [TearDown]
        public void TearDown()
        {

        }

        [UnityTest]
        public IEnumerator ShouldSpawnMapPrefab()
        {
            yield return GameBuilder.Build();

            var mapPrefab = new GameObject("mapPrefab");
            var map = mapPrefab.AddComponent<MapComponent>();

            var locationObject = new GameObject("locationObject");
            locationObject.SetActive(false);
            var location = locationObject.AddComponent<LocationComponent>();
            locationObject.SetActive(true);

            location.MapPrefab = mapPrefab;

        }
    }
}