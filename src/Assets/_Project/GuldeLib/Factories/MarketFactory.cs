using GuldeLib.Economy;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class MarketFactory : Factory<Market, MarketComponent>
    {
        public MarketFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject) { }

        public override MarketComponent Create(Market market)
        {
            Locator.Market = Component;

            var locationFactory = new LocationFactory(GameObject);
            locationFactory.Create(market.Location.Value);

            foreach (var exchange in market.Exchanges)
            {
                var exchangeFactory = new ExchangeFactory(parentObject: GameObject);
                exchangeFactory.Create(exchange.Value);
            }

            return Component;
        }
    }
}