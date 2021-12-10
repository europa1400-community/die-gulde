using GuldeLib.Entities;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class EntityRegistryFactory : Factory<EntityRegistry, EntityRegistryComponent>
    {
        public EntityRegistryFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override EntityRegistryComponent Create(EntityRegistry entityRegistry) => Component;
    }
}