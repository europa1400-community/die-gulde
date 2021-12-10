using System.Collections.Generic;
using GuldeLib.Cities;
using GuldeLib.Economy;
using GuldeLib.Generators;
using GuldeLib.Maps;
using GuldeLib.Timing;
using GuldeLib.TypeObjects;
using GuldeLib.WorkerHomes;

namespace GuldeLib.Builders
{
    public class CityBuilder : Builder<City>
    {
        public CityBuilder WithMap(GeneratableMap map)
        {
            Object.Map = map;
            return this;
        }

        public CityBuilder WithTime(GeneratableTime time)
        {
            Object.Time = time;
            return this;
        }
    }
}