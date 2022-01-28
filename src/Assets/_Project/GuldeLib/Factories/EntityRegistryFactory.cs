using GuldeLib.Entities;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class EntityRegistryFactory : Factory<EntityRegistry, EntityRegistryComponent>
    {
        public EntityRegistryFactory(EntityRegistry entityRegistry, GameObject gameObject = null, GameObject parentObject = null) : base(entityRegistry, gameObject, parentObject)
        {
        }

        public override EntityRegistryComponent Create() => Component;
    }
}