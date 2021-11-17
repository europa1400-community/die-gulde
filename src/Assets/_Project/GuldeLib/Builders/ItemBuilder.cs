using GuldeLib.Economy;
using GuldeLib.Producing;
using MonoLogger.Runtime;
using Sirenix.Utilities;
using UnityEngine;

namespace GuldeLib.Builders
{
    public class ItemBuilder : Builder<Item>
    {
        public ItemBuilder WithName(string name)
        {
            Object.Name = name;
            return this;
        }

        public ItemBuilder WithItemType(ItemType itemType)
        {
            Object.ItemType = itemType;
            return this;
        }

        public ItemBuilder WithMeanPrice(float meanPrice)
        {
            Object.MeanPrice = meanPrice;
            return this;
        }

        public ItemBuilder WithMinPrice(float minPrice)
        {
            Object.MinPrice = minPrice;
            return this;
        }

        public ItemBuilder WithMeanSupply(int meanSupply)
        {
            Object.MeanSupply = meanSupply;
            return this;
        }
    }
}