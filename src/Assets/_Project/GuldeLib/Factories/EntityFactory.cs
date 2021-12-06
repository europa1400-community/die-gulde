using GuldeLib.Entities;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class EntityFactory : Factory<Entity>
    {
        public EntityFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {

        }

        public override GameObject Create(Entity entity)
        {
            if (entity.Naming.Value)
            {
                var namingFactory = new NamingFactory(GameObject);
                namingFactory.Create(entity.Naming.Value);
            }

            var entityComponent = GameObject.AddComponent<EntityComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}