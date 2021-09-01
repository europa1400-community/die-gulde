using Gulde.Economy;
using Gulde.Logging;
using Gulde.Production;
using Sirenix.Utilities;
using UnityEngine;

namespace GuldePlayTests.Builders
{
    public class ItemBuilder
    {
        Item Item { get; }

        public ItemBuilder() => Item = ScriptableObject.CreateInstance<Item>();

        public ItemBuilder WithName(string name)
        {
            var type = Item.GetType();
            var nameProperty = type.GetProperty("Name");

            nameProperty?.SetValue(Item, name);

            return this;
        }

        // public ItemBuilder WithIcon(Sprite icon)
        // {
        //     Item.Icon = icon;
        //
        //     return this;
        // }

        public ItemBuilder WithItemType(ItemType itemType)
        {
            var type = Item.GetType();
            var itemTypeProperty = type.GetProperty("ItemType");

            itemTypeProperty?.SetValue(Item, itemType);

            return this;
        }

        public ItemBuilder WithMeanPrice(float meanPrice)
        {
            var type = Item.GetType();
            var meanPriceProperty = type.GetProperty("MeanPrice");

            meanPriceProperty?.SetValue(Item, meanPrice);

            return this;
        }

        public ItemBuilder WithMinPrice(float minPrice)
        {
            var type = Item.GetType();
            var minPriceProperty = type.GetProperty("MinPrice");

            minPriceProperty?.SetValue(Item, minPrice);

            return this;
        }

        public ItemBuilder WithMeanSupply(int meanSupply)
        {
            var type = Item.GetType();
            var meanSupplyProperty = type.GetProperty("MeanSupply");

            meanSupplyProperty?.SetValue(Item, meanSupply);

            return this;
        }

        public Item Build()
        {
            if (Item.Name.IsNullOrWhitespace())
            {
                this.Log($"Cannot create item with invalid name \"{Item.Name}\"", LogType.Error);
                return null;
            }

            if (Item.MeanPrice <= 0)
            {
                this.Log($"Cannot create Item with invalid mean price {Item.MeanPrice}.", LogType.Error);
                return null;
            }

            if (Item.MinPrice <= 0 || Item.MinPrice > Item.MeanPrice)
            {
                this.Log($"Cannot create Item with invalid min price {Item.MinPrice}.", LogType.Error);
                return null;
            }

            if (Item.MeanSupply <= 0)
            {
                this.Log($"Cannot create Item with invalid mean supply {Item.MeanSupply}.", LogType.Error);
                return null;
            }

            return Item;
        }
    }
}