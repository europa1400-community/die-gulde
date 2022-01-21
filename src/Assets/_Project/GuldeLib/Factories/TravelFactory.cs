using GuldeLib.Entities;
using GuldeLib.Pathfinding;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class TravelFactory : Factory<Travel, TravelComponent>
    {
        public TravelFactory(Travel travel, GameObject gameObject = null, GameObject parentObject = null) : base(travel, gameObject, parentObject)
        {
        }

        public override TravelComponent Create()
        {
            var pathfinderFactory = new PathfinderFactory(TypeObject.Pathfinder.Value, GameObject, ParentObject);
            var pathfinderComponent = pathfinderFactory.Create();

            pathfinderComponent.DestinationReached += Component.OnDestinationReached;

            return Component;
        }
    }
}