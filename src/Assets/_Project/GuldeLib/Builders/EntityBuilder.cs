using GuldeLib.Entities;
using GuldeLib.Entities.Pathfinding;
using GuldeLib.Maps;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Builders
{
    public class EntityBuilder
    {
        public GameObject EntityObject { get; set; }

        string Name { get; set; }
        float Speed { get; set; }
        MapComponent Map { get; set; }

        public EntityBuilder()
        {

        }

        public EntityBuilder WithName(string name)
        {
            Name = name;

            return this;
        }

        public EntityBuilder WithMap(MapComponent map)
        {
            Map = map;

            return this;
        }

        public EntityBuilder WithSpeed(float speed)
        {
            Speed = speed;

            return this;
        }

        public GameObject Build()
        {
            if (Name == "")
            {
                this.Log("Can not create entity without a name.", LogType.Error);
                return null;
            }

            if (!Map)
            {
                this.Log("Can not create entity without a map.", LogType.Error);
                return null;
            }

            EntityObject = new GameObject(Name);

            var entity = EntityObject.AddComponent<EntityComponent>();
            var pathfinding = EntityObject.AddComponent<PathfindingComponent>();

            Map.EntityRegistry.Register(entity);

            pathfinding.Speed = Speed;

            return EntityObject;
        }
    }
}