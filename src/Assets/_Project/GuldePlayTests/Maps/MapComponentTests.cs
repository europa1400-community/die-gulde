// using System.Collections;
// using GuldeLib.Cities;
// using GuldeLib.Entities;
// using NUnit.Framework;
// using UnityEngine.TestTools;
//
// namespace GuldePlayTests.Maps
// {
//     public class MapComponentTests
//     {
//
//         [UnitySetUp]
//         public IEnumerator Setup()
//         {
//
//             yield break;
//         }
//
//         [TearDown]
//         public void Teardown()
//         {
//
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldHandleEntityUnregistering()
//         {
//             var cityBuilder = A.City.WithSize(10, 10);
//             var gameBuilder = A.Game.WithCity(cityBuilder);
//
//             yield return gameBuilder.Build();
//
//             var cityObject = cityBuilder.CityObject;
//             var city = cityObject.GetComponent<CityComponent>();
//
//             var entityObject = A.Entity.WithName("entity").WithMap(city.Map).Build();
//             var entity = entityObject.GetComponent<EntityComponent>();
//
//             city.Map.EntityRegistry.Register(entity);
//
//             Assert.AreEqual(city.Map, entity.Map);
//
//             city.Map.EntityRegistry.Unregister(entity);
//
//             Assert.IsNull(entity.Map);
//         }
//     }
// }