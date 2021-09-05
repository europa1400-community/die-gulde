using System;
using System.Collections.Generic;

namespace GuldeLib.Economy
{
    public class BillingEventArgs : EventArgs
    {
        public BillingEventArgs(Dictionary<TurnoverType, float> expenses, Dictionary<TurnoverType, float> revenues)
        {
            Expenses = expenses;
            Revenues = revenues;
        }

        public Dictionary<TurnoverType, float> Expenses { get; }

        public Dictionary<TurnoverType, float> Revenues { get; }
    }
}