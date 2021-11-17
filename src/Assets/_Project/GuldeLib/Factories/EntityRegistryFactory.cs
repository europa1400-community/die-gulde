using GuldeLib.Entities;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class EntityRegistryFactory : Factory<EntityRegistry>
    {
        public EntityRegistryFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(EntityRegistry entityRegistry)
        {
            var entityRegistryComponent = GameObject.AddComponent<EntityRegistryComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}