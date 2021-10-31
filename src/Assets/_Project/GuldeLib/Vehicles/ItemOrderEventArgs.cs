using System;

namespace GuldeLib.Vehicles
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