using System;
using System.Collections.Generic;

namespace GuldeLib.Economy
{
    /// <summary>
    /// Contains arguments for the <see cref = "WealthComponent.Billed"/> event.
    /// </summary>
    public class BillingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Dictionary mapping expense <see cref = "TurnoverType">TurnoverTypes</see> to the amount of exchanged money.
        /// </summary>
        public Dictionary<TurnoverType, float> Expenses { get; }

        /// <summary>
        /// Gets the Dictionary mapping revenue <see cref = "TurnoverType">TurnoverTypes</see> to the amount of exchanged money.
        /// </summary>
        public Dictionary<TurnoverType, float> Revenues { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref = "BillingEventArgs">BillingEventArgs</see> class.
        /// </summary>
        /// <param name="expenses"><see cref = "BillingEventArgs.Expenses">Expenses Dictionary</see></param>
        /// <param name="revenues"><see cref = "BillingEventArgs.Revenues">Revenues Dictionary</see></param>
        public BillingEventArgs(Dictionary<TurnoverType, float> expenses, Dictionary<TurnoverType, float> revenues)
        {
            Expenses = expenses;
            Revenues = revenues;
        }
    }
}