using GuldeLib.Entities;
using GuldeLib.Generators;
using GuldeLib.Pathfinding;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class TravelBuilder : Builder<Travel>
    {
        public TravelBuilder WithPathfinder(GeneratablePathfinder pathfinder)
        {
            Object.Pathfinder = pathfinder;
            return this;
        }
    }
}