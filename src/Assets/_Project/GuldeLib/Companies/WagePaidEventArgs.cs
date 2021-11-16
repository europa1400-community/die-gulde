using System;

namespace GuldeLib.Companies
{
    /// <summary>
    /// Contains arguments for the <see cref = "CompanyComponent.WagePaid"/> event.
    /// </summary>
    public class WagePaidEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the cost of the paid wages.
        /// </summary>
        public float Cost { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref = "WagePaidEventArgs">WagePaidEventArgs</see> class.
        /// </summary>
        /// <param name="cost">The cost of the paid wages</param>
        public WagePaidEventArgs(float cost)
        {
            Cost = cost;
        }
    }
}