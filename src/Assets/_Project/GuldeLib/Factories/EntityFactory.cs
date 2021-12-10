using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.TypeObjects;
using MonoExtensions.Runtime;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class EntityFactory : Factory<Entity, EntityComponent>
    {
        public EntityFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {

        }

        public override EntityComponent Create(Entity entity)
        {
            if (entity.Naming?.Value)
            {
                var namingFactory = new NamingFactory(GameObject);
                namingFactory.Create(entity.Naming.Value);
            }

            var parentLocationComponent = ParentObject.GetComponent<LocationComponent>();
            if (parentLocationComponent) Component.Position = parentLocationComponent.EntryCell.ToWorld();

            return Component;
        }
    }
}