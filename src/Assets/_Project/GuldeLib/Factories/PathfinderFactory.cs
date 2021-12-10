using GuldeLib.Pathfinding;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class PathfinderFactory : Factory<Pathfinder, PathfinderComponent>
    {
        public PathfinderFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override PathfinderComponent Create(Pathfinder pathfinder)
        {
            this.Log($"Creating pathfinder object.");
            Component.Speed = pathfinder.Speed.Value;

            var entityFactory = new EntityFactory(GameObject, ParentObject);
            entityFactory.Create(pathfinder.Entity.Value);

            this.Log($"Finished creating pathfinder object.");
            return Component;
        }
    }
}