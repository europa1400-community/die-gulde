using GuldeLib.Entities;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class TravelFactory : Factory<Travel>
    {
        public TravelFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Travel travel)
        {
            var pathfinderFactory = new PathfinderFactory(GameObject);
            pathfinderFactory.Create(travel.Pathfinder);

            var travelComponent = GameObject.AddComponent<TravelComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}