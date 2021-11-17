using GuldeLib.Economy;
using GuldeLib.Inventories;
using GuldeLib.Names;

namespace GuldeLib.Builders
{
    public class ExchangeBuilder : Builder<Exchange>
    {
        public ExchangeBuilder WithNaming(Naming naming)
        {
            Object.Naming = naming;
            return this;
        }

        public ExchangeBuilder WithInventory(Inventory inventory)
        {
            Object.Inventory = inventory;
            return this;
        }

        public ExchangeBuilder WithProductInventory(Inventory productInventory)
        {
            Object.ProductInventory = productInventory;
            return this;
        }

        public ExchangeBuilder WithIsPurchasing(bool isPurchasing)
        {
            Object.IsPurchasing = isPurchasing;
            return this;
        }

        public ExchangeBuilder WithIsSelling(bool isSelling)
        {
            Object.IsSelling = isSelling;
            return this;
        }
    }
}