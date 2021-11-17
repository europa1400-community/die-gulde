using GuldeLib.Cities;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class CityFactory : Factory<City>
    {
        public CityFactory() : base(null, null)
        {
        }

        public override GameObject Create(City city)
        {
            var timeFactory = new TimeFactory(GameObject);
            timeFactory.Create(city.Time);

            var mapFactory = new MapFactory(GameObject);
            mapFactory.Create(city.Map);

            var marketFactory = new MarketFactory(parentObject: GameObject);
            marketFactory.Create(city.Market);

            var cityComponent = GameObject.AddComponent<CityComponent>();

            foreach (var workerHome in city.WorkerHomes)
            {
                var workerHomeFactory = new WorkerHomeFactory(parentObject: GameObject);
                workerHomeFactory.Create(workerHome);
            }

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}