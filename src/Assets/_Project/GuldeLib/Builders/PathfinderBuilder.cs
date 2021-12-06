using GuldeLib.Entities;
using GuldeLib.Generators;
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

        public PathfinderBuilder WithEntity(GeneratableEntity entity)
        {
            Object.Entity = entity;
            return this;
        }
    }
}