using GuldeLib.Cities;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class CityFactory : Factory<City>
    {
        public CityFactory(GameObject parentObject) : base(null, parentObject)
        {
        }

        public override GameObject Create(City city)
        {
            var timeFactory = new TimeFactory(GameObject);
            timeFactory.Create(city.Time.Value);

            var mapFactory = new MapFactory(GameObject);
            mapFactory.Create(city.Map.Value);

            var marketFactory = new MarketFactory(parentObject: GameObject);
            marketFactory.Create(city.Market.Value);

            var cityComponent = GameObject.AddComponent<CityComponent>();

            foreach (var workerHome in city.WorkerHomes)
            {
                var workerHomeFactory = new WorkerHomeFactory(parentObject: GameObject);
                workerHomeFactory.Create(workerHome.Value);
            }

            foreach (var company in city.Companies)
            {
                var companyFactory = new CompanyFactory(parentObject: GameObject);
                companyFactory.Create(company.Value);
            }

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}