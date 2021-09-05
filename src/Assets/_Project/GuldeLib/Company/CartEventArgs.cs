using System;
using GuldeLib.Vehicles;

namespace GuldeLib.Company
{
    public class CartEventArgs : EventArgs
    {
        public CartEventArgs(CartComponent cart)
        {
            Cart = cart;
        }

        public CartComponent Cart { get; }
    }
}