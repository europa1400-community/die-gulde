using System;

namespace GuldeLib.Companies.Carts
{
    public class ItemOrderEventArgs : EventArgs
    {
        public ItemOrderEventArgs(ItemOrder order)
        {
            Order = order;
        }

        ItemOrder Order { get; }
    }
}