using GuldeLib.Cities;
using GuldeLib.Maps;
using GuldeLib.TypeObjects;
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
            var mapComponent = GameObject.GetComponent<MapComponent>();

            var marketFactory = new MarketFactory(parentObject: GameObject);
            marketFactory.Create(city.Market.Value);

            var cityComponent = GameObject.AddComponent<CityComponent>();

            mapComponent.LocationRegistered += cityComponent.OnLocationRegistered;

            foreach (var workerHome in city.WorkerHomes)
            {
                var workerHomeFactory = new WorkerHomeFactory(parentObject: GameObject);
                var workerHomeObject = workerHomeFactory.Create(workerHome.Value);
                var locationComponent = workerHomeObject.GetComponent<LocationComponent>();
                mapComponent.Register(locationComponent);
            }

            foreach (var company in city.Companies)
            {
                var companyFactory = new CompanyFactory(parentObject: GameObject);
                var companyObject = companyFactory.Create(company.Value);
                var locationComponent = companyObject.GetComponent<LocationComponent>();
                mapComponent.Register(locationComponent);
            }

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}