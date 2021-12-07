using GuldeLib.Economy;
using GuldeLib.Generators;
using GuldeLib.Inventories;
using GuldeLib.Names;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class ExchangeBuilder : Builder<Exchange>
    {
        public ExchangeBuilder WithNaming(GeneratableNaming naming)
        {
            Object.Naming = naming;
            return this;
        }

        public ExchangeBuilder WithInventory(GeneratableInventory inventory)
        {
            Object.Inventory = inventory;
            return this;
        }

        public ExchangeBuilder WithProductInventory(GeneratableInventory productInventory)
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