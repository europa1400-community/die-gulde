using GuldeLib.Cities;
using GuldeLib.Maps;
using GuldeLib.Timing;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class CityFactory : Factory<City, CityComponent>
    {
        public CityFactory(GameObject parentObject) : base(null, parentObject)
        {
        }

        public override CityComponent Create(City city)
        {
            Locator.City = Component;

            if (city.Naming?.Value)
            {
                var namingFactory = new NamingFactory(GameObject);
                namingFactory.Create(city.Naming.Value);
            }

            var timeFactory = new TimeFactory(GameObject);
            timeFactory.Create(city.Time.Value);

            var mapFactory = new MapFactory(GameObject);
            mapFactory.Create(city.Map.Value);

            var timeComponent = GameObject.GetComponent<TimeComponent>();
            timeComponent.StartTime();

            return Component;
        }
    }
}