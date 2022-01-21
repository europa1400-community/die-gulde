using GuldeLib.Economy;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class MarketFactory : Factory<Market, MarketComponent>
    {
        public MarketFactory(Market market, GameObject gameObject = null, GameObject parentObject = null) : base(market, gameObject, parentObject) { }

        public override MarketComponent Create()
        {
            Locator.Market = Component;

            var locationFactory = new LocationFactory(TypeObject.Location.Value, GameObject);
            locationFactory.Create();

            foreach (var exchange in TypeObject.Exchanges)
            {
                var exchangeFactory = new ExchangeFactory(exchange.Value, parentObject: GameObject);
                exchangeFactory.Create();
            }

            return Component;
        }
    }
}