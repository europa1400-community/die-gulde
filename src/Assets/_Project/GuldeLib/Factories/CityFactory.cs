using GuldeLib.Cities;
using GuldeLib.Maps;
using GuldeLib.Timing;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class CityFactory : Factory<City, CityComponent>
    {
        public CityFactory(City city, GameObject parentObject) : base(city, null, parentObject)
        {
        }

        public override CityComponent Create()
        {
            Locator.City = Component;

            if (TypeObject.Naming?.Value)
            {
                var namingFactory = new NamingFactory(TypeObject.Naming.Value, GameObject);
                namingFactory.Create();
            }

            var timeFactory = new TimeFactory(TypeObject.Time.Value, GameObject);
            timeFactory.Create();

            var mapFactory = new MapFactory(TypeObject.Map.Value, GameObject);
            mapFactory.Create();

            var timeComponent = GameObject.GetComponent<TimeComponent>();
            timeComponent.StartTime();

            return Component;
        }
    }
}