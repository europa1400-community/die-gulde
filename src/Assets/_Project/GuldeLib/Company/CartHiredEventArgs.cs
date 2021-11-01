using System;
using GuldeLib.Company.Employees;
using GuldeLib.Entities;
using GuldeLib.Vehicles;

namespace GuldeLib.Company
{
    /// <summary>
    /// Contains arguments for the <see cref = "CompanyComponent.CartHired">CartHired</see> event
    /// of the <see cref = "CompanyComponent">CompanyComponent</see>.
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