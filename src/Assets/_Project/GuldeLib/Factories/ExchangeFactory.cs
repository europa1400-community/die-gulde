using GuldeLib.Economy;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ExchangeFactory : Factory<Exchange>
    {
        public ExchangeFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject) { }

        public override GameObject Create(Exchange exchange)
        {
            if (exchange.Naming)
            {
                var namingFactory = new NamingFactory(GameObject);
                namingFactory.Create(exchange.Naming);
            }

            var inventoryFactory = new InventoryFactory(GameObject);
            inventoryFactory.Create(exchange.Inventory);

            if (exchange.ProductInventory)
            {
                var productInventoryFactory = new InventoryFactory(GameObject);
                productInventoryFactory.Create(exchange.ProductInventory);
            }

            var exchangeComponent = GameObject.AddComponent<ExchangeComponent>();
            exchangeComponent.IsPurchasing = exchange.IsPurchasing;
            exchangeComponent.IsSelling = exchange.IsSelling;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}