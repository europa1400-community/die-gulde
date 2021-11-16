using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.Pathfinding;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Builders
{
    /// <summary>
    /// Provides functionality to build an entity.
    /// </summary>
    public class EntityBuilder
    {
        /// <summary>
        /// Gets the built entity's <see cref = "GameObject">GameObject</see>.
        /// </summary>
        public GameObject EntityObject { get; private set; }

        /// <inheritdoc cref="EntityComponent.Name"/>summary>
        string Name { get; set; }

        /// <inheritdoc cref="PathfindingComponent.Speed"/>
        float Speed { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "MapComponent">Map</see> the built entity exists on.
        /// </summary>
        MapComponent Map { get; set; }

        /// <summary>
        /// Sets the <see cref = "EntityComponent.Name">Name</see> of the built entity.
        /// </summary>
        public EntityBuilder WithName(string name)
        {
            Name = name;

            return this;
        }

        /// <summary>
        /// Sets the <see cref = "MapComponent">Map</see> the built entity exits on.
        /// </summary>
        public EntityBuilder WithMap(MapComponent map)
        {
            Map = map;

            return this;
        }

        /// <summary>
        /// Sets the <see cref = "PathfindingComponent.Speed">Speed</see> of the built entity.
        /// </summary>
        public EntityBuilder WithSpeed(float speed)
        {
            Speed = speed;

            return this;
        }

        /// <inheritdoc cref="Builder.Build"/>
        public GameObject Build()
        {
            if (Name == null)
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