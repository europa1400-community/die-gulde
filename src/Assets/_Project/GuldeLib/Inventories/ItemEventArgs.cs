using System;
using GuldeLib.Economy;
using GuldeLib.TypeObjects;

namespace GuldeLib.Inventories
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