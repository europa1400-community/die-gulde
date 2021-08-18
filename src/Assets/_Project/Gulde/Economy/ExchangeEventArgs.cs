using System;

namespace Gulde.Economy
{
    public class ExchangeEventArgs : EventArgs
    {
        public ExchangeEventArgs(Item item, float price)
        {
            Item = item;
            Price = price;
        }

        public Item Item { get; }
        public float Price { get; }
    }
}