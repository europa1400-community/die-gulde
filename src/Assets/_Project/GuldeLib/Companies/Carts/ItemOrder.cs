using System;
using GuldeLib.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Companies.Carts
{
    [Serializable]
    public class ItemOrder
    {
        public ItemOrder(Item item, int amount)
        {
            Item = item;
            Amount = amount;
        }

        [OdinSerialize]
        [ShowInInspector]
        public Item Item { get; }

        [OdinSerialize]
        public int Amount { get; set; }
    }
}