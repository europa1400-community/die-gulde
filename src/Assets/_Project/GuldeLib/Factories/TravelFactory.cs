using GuldeLib.Entities;
using GuldeLib.Pathfinding;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class TravelFactory : Factory<Travel, TravelComponent>
    {
        public TravelFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override TravelComponent Create(Travel travel)
        {
            var pathfinderFactory = new PathfinderFactory(GameObject, ParentObject);
            var pathfinderComponent = pathfinderFactory.Create(travel.Pathfinder.Value);

            pathfinderComponent.DestinationReached += Component.OnDestinationReached;

            return Component;
        }
    }
}