using System;
using GuldeLib.Companies.Carts;

namespace GuldeLib.Companies
{
    /// <summary>
    /// Contains arguments for the <see cref = "CompanyComponent.CartLeft"/> event.
    /// </summary>
    public class CartLeftEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref = "CartComponent">Cart</see> which has left the company.
        /// </summary>
        public CartComponent Cart { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref = "CartLeftEventArgs">CartLeftEventArgs</see> class.
        /// </summary>
        /// <param name="cart">The <see cref = "CartComponent">Cart</see> which has left the company.</param>
        public CartLeftEventArgs(CartComponent cart)
        {
            Cart = cart;
        }
    }
}