using System;
using GuldeLib.Companies.Carts;

namespace GuldeLib.Companies
{
    /// <summary>
    /// Contains arguments for the <see cref = "CompanyComponent.CartHired"/> event.
    /// </summary>
    public class CartHiredEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the hired <see cref = "CartComponent">Cart</see>.
        /// </summary>
        public CartComponent Cart { get; }

        /// <summary>
        /// Gets the cost of hiring the cart.
        /// </summary>
        public int Cost { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref = "CartHiredEventArgs">CartHiredEventArgs</see> class.
        /// </summary>
        /// <param name="cart">The hired <see cref = "CartComponent">Cart</see>.</param>
        /// <param name="cost">The cost of hiring the cart.</param>
        public CartHiredEventArgs(CartComponent cart, int cost)
        {
            Cart = cart;
            Cost = cost;
        }
    }
}