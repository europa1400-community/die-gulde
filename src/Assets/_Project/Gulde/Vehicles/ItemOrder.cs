using System;
using Gulde.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gulde.Vehicles
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