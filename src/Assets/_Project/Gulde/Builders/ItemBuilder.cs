using Gulde.Economy;
using Gulde.Production;
using UnityEngine;

namespace Gulde.Builders
{
    public class ItemBuilder
    {
        Item Item { get; set; }

        public ItemBuilder() => Item = ScriptableObject.CreateInstance<Item>();

        public ItemBuilder WithName(string name)
        {
            Item.Name = name;

            return this;
        }

        public ItemBuilder WithIcon(Sprite icon)
        {
            Item.Icon = icon;

            return this;
        }

        public ItemBuilder WithItemType(ItemType itemType)
        {
            Item.ItemType = itemType;

            return this;
        }

        public ItemBuilder WithMeanPrice(float meanPrice)
        {
            Item.MeanPrice = meanPrice;

            return this;
        }

        public ItemBuilder WithMinPrice(float minPrice)
        {
            Item.MinPrice = minPrice;

            return this;
        }

        public ItemBuilder WithMeanSupply(int meanSupply)
        {
            Item.MeanSupply = meanSupply;

            return this;
        }

        public Item Build() => Item;
    }
}