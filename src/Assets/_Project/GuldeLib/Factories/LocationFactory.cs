using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.TypeObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class LocationFactory : Factory<Location, LocationComponent>
    {
        public LocationFactory(Location location, GameObject gameObject = null, GameObject parentObject = null) : base(location, gameObject, parentObject) { }

        public override LocationComponent Create()
        {
            if (TypeObject.Naming?.Value)
            {
                var namingFactory = new NamingFactory(TypeObject.Naming.Value, GameObject);
                namingFactory.Create();
            }

            Component.MapPrefab = TypeObject.MapPrefab;

            var entityRegistryFactory = new EntityRegistryFactory(TypeObject.EntityRegistry.Value, GameObject);
            var entityRegistryComponent = entityRegistryFactory.Create();

            entityRegistryComponent.Registered += Component.OnEntityRegistered;
            entityRegistryComponent.Unregistered += Component.OnEntityUnregistered;

            if (TypeObject.Building?.Value)
            {
                var buildingFactory = new BuildingFactory(TypeObject.Building.Value, GameObject);
                buildingFactory.Create();
            }

            return Component;
        }
    }
}