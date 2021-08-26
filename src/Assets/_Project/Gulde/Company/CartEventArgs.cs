using System;
using Gulde.Vehicles;

namespace Gulde.Company
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