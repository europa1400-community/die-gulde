using GuldeLib.Entities;
using GuldeLib.Pathfinding;

namespace GuldeLib.Builders
{
    public class PathfinderBuilder : Builder<Pathfinder>
    {
        public PathfinderBuilder WithSpeed(float speed)
        {
            Object.Speed = speed;
            return this;
        }

        public PathfinderBuilder WithEntity(Entity entity)
        {
            Object.Entity = entity;
            return this;
        }
    }
}