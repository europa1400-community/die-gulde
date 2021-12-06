using GuldeLib.Pathfinding;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class PathfinderFactory : Factory<Pathfinder>
    {
        public PathfinderFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Pathfinder pathfinder)
        {
            var entityFactory = new EntityFactory(GameObject);
            entityFactory.Create(pathfinder.Entity.Value);

            var pathfinderComponent = GameObject.AddComponent<PathfinderComponent>();

            pathfinderComponent.Speed = pathfinder.Speed.Value;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}