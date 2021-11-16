using GuldeLib.Entities;
using GuldeLib.Maps;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class LocationFactory : Factory<Location>
    {
        public LocationFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject) { }

        public override GameObject Create(Location location)
        {
            var namingFactory = new NamingFactory(GameObject);
            namingFactory.Create(location.Naming);

            GameObject.AddComponent<EntityRegistryComponent>();

            var locationComponent = GameObject.AddComponent<LocationComponent>();

            locationComponent.EntryCell = location.EntryCell;
            locationComponent.MapPrefab = location.MapPrefab;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}