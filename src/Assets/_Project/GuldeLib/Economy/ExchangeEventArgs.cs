using System;
using GuldeLib.TypeObjects;

namespace GuldeLib.Economy
{
    public class ExchangeEventArgs : EventArgs
    {
        public ExchangeEventArgs(Item item, float price, int amount = 1)
        {
            Item = item;
            Price = price;
            Amount = amount;
        }

        public Item Item { get; }
        public float Price { get; }
        public int Amount { get; }
    }
}