using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.TypeObjects;
using MonoExtensions.Runtime;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class EntityFactory : Factory<Entity, EntityComponent>
    {
        public EntityFactory(Entity entity, GameObject gameObject = null, GameObject parentObject = null) : base(entity, gameObject, parentObject)
        {

        }

        public override EntityComponent Create()
        {
            if (TypeObject.Naming?.Value)
            {
                var namingFactory = new NamingFactory(TypeObject.Naming.Value, GameObject);
                namingFactory.Create();
            }

            var parentLocationComponent = ParentObject.GetComponent<LocationComponent>();
            if (parentLocationComponent) Component.Position = parentLocationComponent.EntryCell.ToWorld();

            return Component;
        }
    }
}