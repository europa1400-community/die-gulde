// using GuldeLib.Entities;
// using GuldePlayTests;
// using NUnit.Framework;
// using UnityEngine;
//
// namespace GuldeTests.Entities
// {
//     public class EntityRegistryTests
//     {
//         bool RegisteredFlag { get; set; }
//         bool UnregisteredFlag { get; set; }
//
//         [TearDown]
//         public void Teardown()
//         {
//             RegisteredFlag = false;
//             UnregisteredFlag = false;
//         }
//
//         [Test]
//         public void ShouldRegisterAndUnregisterEntity()
//         {
//             var gameObject = new GameObject("registry");
//             var registry = gameObject.AddComponent<EntityRegistryComponent>();
//
//             var entityObject = new GameObject("entity");
//             var entity = entityObject.AddComponent<EntityComponent>();
//
//             registry.Registered += OnRegistered;
//             registry.Unregistered += OnUnregistered;
//
//             registry.Register(entity);
//
//             Assert.True(RegisteredFlag);
//             Assert.False(UnregisteredFlag);
//             Assert.True(registry.IsRegistered(entity));
//             CollectionAssert.Contains(registry.Entities, entity);
//
//             RegisteredFlag = false;
//
//             registry.Unregister(entity);
//
//             Assert.False(RegisteredFlag);
//             Assert.True(UnregisteredFlag);
//             Assert.False(registry.IsRegistered(entity));
//             CollectionAssert.DoesNotContain(registry.Entities, entity);
//         }
//
//         [Test]
//         public void ShouldNotRegisterOrUnregisterInvalidEntity()
//         {
//             var gameObject = new GameObject("registry");
//             var registry = gameObject.AddComponent<EntityRegistryComponent>();
//
//             registry.Registered += OnRegistered;
//             registry.Unregistered += OnUnregistered;
//
//             registry.Register(null);
//
//             Assert.False(RegisteredFlag);
//             Assert.False(UnregisteredFlag);
//             Assert.False(registry.IsRegistered(null));
//             CollectionAssert.DoesNotContain(registry.Entities, null);
//
//             RegisteredFlag = false;
//
//             registry.Unregister(null);
//
//             Assert.False(RegisteredFlag);
//             Assert.False(UnregisteredFlag);
//             Assert.False(registry.IsRegistered(null));
//             CollectionAssert.DoesNotContain(registry.Entities, null);
//         }
//
//         void OnRegistered(object sender, EntityRegistryComponent.EntityEventArgs e)
//         {
//             RegisteredFlag = true;
//         }
//
//         void OnUnregistered(object sender, EntityRegistryComponent.EntityEventArgs e)
//         {
//             UnregisteredFlag = true;
//         }
//     }
// }