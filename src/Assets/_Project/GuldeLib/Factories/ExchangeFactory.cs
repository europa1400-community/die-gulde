using GuldeLib.Economy;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ExchangeFactory : Factory<Exchange, ExchangeComponent>
    {
        public ExchangeFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject) { }

        public override ExchangeComponent Create(Exchange exchange)
        {
            if (exchange.Naming?.Value)
            {
                var namingFactory = new NamingFactory(GameObject);
                namingFactory.Create(exchange.Naming.Value);
            }

            var inventoryFactory = new InventoryFactory(GameObject, allowMultiple: true);
            inventoryFactory.Create(exchange.Inventory.Value);

            if (exchange.ProductInventory?.Value)
            {
                var productInventoryFactory = new InventoryFactory(GameObject, allowMultiple: true);
                productInventoryFactory.Create(exchange.ProductInventory.Value);
            }

            Component.IsPurchasing = exchange.IsPurchasing;
            Component.IsSelling = exchange.IsSelling;

            return Component;
        }
    }
}