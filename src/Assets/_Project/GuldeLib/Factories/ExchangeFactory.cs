using GuldeLib.Economy;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ExchangeFactory : Factory<Exchange, ExchangeComponent>
    {
        public ExchangeFactory(Exchange exchange, GameObject gameObject = null, GameObject parentObject = null) : base(exchange, gameObject, parentObject) { }

        public override ExchangeComponent Create()
        {
            if (TypeObject.Naming?.Value)
            {
                var namingFactory = new NamingFactory(TypeObject.Naming.Value, GameObject);
                namingFactory.Create();
            }

            var inventoryFactory = new InventoryFactory(TypeObject.Inventory.Value, GameObject, allowMultiple: true);
            inventoryFactory.Create();

            if (TypeObject.ProductInventory?.Value)
            {
                var productInventoryFactory = new InventoryFactory(TypeObject.ProductInventory.Value, GameObject, allowMultiple: true);
                productInventoryFactory.Create();
            }

            Component.IsPurchasing = TypeObject.IsPurchasing;
            Component.IsSelling = TypeObject.IsSelling;

            return Component;
        }
    }
}