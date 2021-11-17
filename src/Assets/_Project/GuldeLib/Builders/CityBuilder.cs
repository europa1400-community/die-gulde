using System.Collections.Generic;
using GuldeLib.Cities;
using GuldeLib.Economy;
using GuldeLib.Maps;
using GuldeLib.Timing;
using GuldeLib.WorkerHomes;

namespace GuldeLib.Builders
{
    public class CityBuilder : Builder<City>
    {
        public CityBuilder WithMap(Map map)
        {
            Object.Map = map;
            return this;
        }

        public CityBuilder WithTime(Time time)
        {
            Object.Time = time;
            return this;
        }

        public CityBuilder WithMarket(Market market)
        {
            Object.Market = market;
            return this;
        }

        public CityBuilder WithWorkerHomes(List<WorkerHome> workerHomes)
        {
            Object.WorkerHomes = workerHomes;
            return this;
        }
    }
}