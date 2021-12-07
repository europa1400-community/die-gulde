using System.Collections.Generic;
using GuldeLib.Economy;
using GuldeLib.Generators;
using GuldeLib.Maps;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Builders
{
    public class MarketBuilder : Builder<Market>
    {
        public MarketBuilder WithExchanges(List<GeneratableExchange> exchanges)
        {
            Object.Exchanges = exchanges;
            return this;
        }

        public MarketBuilder WithLocation(GeneratableLocation location)
        {
            Object.Location = location;
            return this;
        }
    }
}