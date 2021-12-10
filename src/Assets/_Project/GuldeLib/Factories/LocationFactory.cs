using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.TypeObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class LocationFactory : Factory<Location, LocationComponent>
    {
        public LocationFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject) { }

        public override LocationComponent Create(Location location)
        {
            if (location.Naming?.Value)
            {
                var namingFactory = new NamingFactory(GameObject);
                namingFactory.Create(location.Naming.Value);
            }

            Component.MapPrefab = location.MapPrefab;

            var entityRegistryFactory = new EntityRegistryFactory(GameObject);
            var entityRegistryComponent = entityRegistryFactory.Create(location.EntityRegistry.Value);

            entityRegistryComponent.Registered += Component.OnEntityRegistered;
            entityRegistryComponent.Unregistered += Component.OnEntityUnregistered;

            if (location.Building?.Value)
            {
                var buildingFactory = new BuildingFactory(GameObject);
                buildingFactory.Create(location.Building.Value);
            }

            return Component;
        }
    }
}