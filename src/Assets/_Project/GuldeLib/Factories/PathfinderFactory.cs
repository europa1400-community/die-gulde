using GuldeLib.Pathfinding;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class PathfinderFactory : Factory<Pathfinder, PathfinderComponent>
    {
        public PathfinderFactory(Pathfinder pathfinder, GameObject gameObject = null, GameObject parentObject = null) : base(pathfinder, gameObject, parentObject)
        {
        }

        public override PathfinderComponent Create()
        {
            this.Log($"Creating pathfinder object.");
            Component.Speed = TypeObject.Speed.Value;

            var entityFactory = new EntityFactory(TypeObject.Entity.Value, GameObject, ParentObject);
            entityFactory.Create();

            this.Log($"Finished creating pathfinder object.");
            return Component;
        }
    }
}