using System;

namespace Gulde.Company
{
    public class CostEventArgs : EventArgs
    {
        public CostEventArgs(float cost)
        {
            Cost = cost;
        }

        public float Cost { get; }
    }
}