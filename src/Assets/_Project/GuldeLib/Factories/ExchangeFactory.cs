using GuldeLib.Economy;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ExchangeFactory : Factory<Exchange>
    {
        public ExchangeFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject) { }

        public override GameObject Create(Exchange exchange)
        {
            if (exchange.Naming?.Value)
            {
                var namingFactory = new NamingFactory(GameObject);
                namingFactory.Create(exchange.Naming.Value);
            }

            var inventoryFactory = new InventoryFactory(GameObject);
            inventoryFactory.Create(exchange.Inventory.Value);

            if (exchange.ProductInventory?.Value)
            {
                var productInventoryFactory = new InventoryFactory(GameObject);
                productInventoryFactory.Create(exchange.ProductInventory.Value);
            }

            var exchangeComponent = GameObject.AddComponent<ExchangeComponent>();
            exchangeComponent.IsPurchasing = exchange.IsPurchasing;
            exchangeComponent.IsSelling = exchange.IsSelling;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}