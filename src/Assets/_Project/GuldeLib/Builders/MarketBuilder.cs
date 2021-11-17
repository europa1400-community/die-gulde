using System.Collections.Generic;
using GuldeLib.Economy;
using GuldeLib.Maps;

namespace GuldeLib.Builders
{
    public class MarketBuilder : Builder<Market>
    {
        public MarketBuilder WithExchanges(List<Exchange> exchanges)
        {
            Object.Exchanges = exchanges;
            return this;
        }

        public MarketBuilder WithLocation(Location location)
        {
            Object.Location = location;
            return this;
        }
    }
}