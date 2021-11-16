using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GuldeLib;
using GuldeLib.Builders;
using GuldeLib.Cities;
using GuldeLib.Economy;
using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.Pathfinding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Maps
{
    public class LocationComponentTests
    {
        GameBuilder GameBuilder { get; set; }
        CityBuilder CityBuilder { get; set; }

        GameObject CityObject => CityBuilder.CityObject;
        CityComponent City => CityObject.GetComponent<CityComponent>();
        
        MapComponent ChangedMap { get; set; }
        bool ContainingMapChangedFlag { get; set; }

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
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            ContainingMapChangedFlag = false;
            ChangedMap = null;
        }

        [UnityTest]
        public IEnumerator ShouldSpawnMapPrefab()
        {
            yield return GameBuilder.Build();

            var mapPrefab = new GameObject("mapPrefab");
            mapPrefab.SetActive(false);
            var nav = mapPrefab.AddComponent<NavComponent>();
            mapPrefab.SetActive(true);

            var locationObject = new GameObject("locationObject");
            locationObject.SetActive(false);
            var location = locationObject.AddComponent<LocationComponent>();
            location.ContainingMapChanged += OnContainingMapChanged;
            location.SetContainingMap(City.Map);
            location.MapPrefab = mapPrefab;
            locationObject.SetActive(true);

            Assert.True(ContainingMapChangedFlag);
            Assert.NotNull(location.AssociatedMap);
            Assert.NotNull(ChangedMap);
            Assert.AreEqual(City.Map, location.ContainingMap);
            Assert.AreEqual(City.Map, ChangedMap);
        }

        void OnContainingMapChanged(object sender, MapEventArgs e)
        {
            ContainingMapChangedFlag = true;
            ChangedMap = e.Map;
        }

        [UnityTest]
        public IEnumerator ShouldGetChildExchanges()
        {
            yield return GameBuilder.Build();

            var marketObject = new GameObject("marketObject");
            var subMarketObject1 = new GameObject("subMarketObject1");
            var subMarketObject2 = new GameObject("subMarketObject2");

            subMarketObject1.transform.SetParent(marketObject.transform);
            subMarketObject2.transform.SetParent(marketObject.transform);
            
            marketObject.SetActive(false);
            subMarketObject1.SetActive(false);
            subMarketObject2.SetActive(false);

            var exchange1 = subMarketObject1.AddComponent<ExchangeComponent>();
            var exchange2 = subMarketObject2.AddComponent<ExchangeComponent>();

            var location = marketObject.AddComponent<LocationComponent>();

            marketObject.SetActive(true);
            subMarketObject1.SetActive(true);
            subMarketObject2.SetActive(true);
            
            CollectionAssert.AreEqual(new List<ExchangeComponent>() { exchange1, exchange2 }, location.Exchanges);
        }
        
        [UnityTest]
        public IEnumerator ShouldRegisterAndUnregisterEntitiesInAssociatedMap()
        {
            yield return GameBuilder.Build();

            var mapPrefab = new GameObject("mapPrefab");
            mapPrefab.SetActive(false);
            var nav = mapPrefab.AddComponent<NavComponent>();
            mapPrefab.SetActive(true);

            var locationObject = new GameObject("locationObject");
            locationObject.SetActive(false);
            var location = locationObject.AddComponent<LocationComponent>();
            location.SetContainingMap(City.Map);
            location.MapPrefab = mapPrefab;
            locationObject.SetActive(true);

            var entityObject = A.Entity.WithMap(City.Map).WithName("entity").Build();
            var entity = entityObject.GetComponent<EntityComponent>();
            location.EntityRegistry.Register(entity);
            
            Assert.AreEqual(location.AssociatedMap, entity.Map);
            Assert.AreEqual(location, entity.Location);
            
            location.EntityRegistry.Unregister(entity);
            
            Assert.AreEqual(Locator.City.Map, entity.Map);
            Assert.AreEqual(location.ContainingMap, entity.Map);
            Assert.IsNull(entity.Location);
        }
        
        
    }
}