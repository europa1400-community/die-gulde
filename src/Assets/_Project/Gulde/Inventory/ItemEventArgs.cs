using System;
using Gulde.Economy;

namespace Gulde.Inventory
{
    public class ItemEventArgs : EventArgs
    {
        public ItemEventArgs(Item item, int supply)
        {
            Item = item;
            Supply = supply;
        }

        public Item Item { get; }
        public int Supply { get; }
    }
}