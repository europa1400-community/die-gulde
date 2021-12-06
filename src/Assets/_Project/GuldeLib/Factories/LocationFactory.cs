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
            if (location.Naming.Value)
            {
                var namingFactory = new NamingFactory(GameObject);
                namingFactory.Create(location.Naming.Value);
            }

            GameObject.AddComponent<EntityRegistryComponent>();

            var locationComponent = GameObject.AddComponent<LocationComponent>();

            locationComponent.EntryCell = location.EntryCell.Value;
            locationComponent.MapPrefab = location.MapPrefab;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}