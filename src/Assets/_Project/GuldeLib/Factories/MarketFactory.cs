using GuldeLib.Economy;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class MarketFactory : Factory<Market>
    {
        public MarketFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject) { }

        public override GameObject Create(Market market)
        {
            var locationFactory = new LocationFactory(GameObject);
            locationFactory.Create(market.Location.Value);

            foreach (var exchange in market.Exchanges)
            {
                var exchangeFactory = new ExchangeFactory(parentObject: GameObject);
                exchangeFactory.Create(exchange.Value);
            }

            GameObject.AddComponent<MarketComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}