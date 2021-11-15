using System;
using GuldeLib.Vehicles;

namespace GuldeLib.Company
{
    /// <summary>
    /// Contains arguments for the <see cref = "CompanyComponent.CartArrived"/> event.
    /// </summary>
    public class CartArrivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref = "CartComponent">Cart</see> which has arrived at the company.
        /// </summary>
        public CartComponent Cart { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref = "CartArrivedEventArgs">CartArrivedEventArgs</see> class.
        /// </summary>
        /// <param name="cart">The <see cref = "CartComponent">Cart</see> which has arrived at the company.</param>
        public CartArrivedEventArgs(CartComponent cart)
        {
            Cart = cart;
        }
    }
}