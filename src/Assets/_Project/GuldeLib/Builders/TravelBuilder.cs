using GuldeLib.Entities;
using GuldeLib.Pathfinding;

namespace GuldeLib.Builders
{
    public class TravelBuilder : Builder<Travel>
    {
        public TravelBuilder WithPathfinder(Pathfinder pathfinder)
        {
            Object.Pathfinder = pathfinder;
            return this;
        }
    }
}