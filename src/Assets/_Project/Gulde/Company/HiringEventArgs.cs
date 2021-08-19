using System;
using Gulde.Entities;

namespace Gulde.Company
{
    public class HiringEventArgs : EventArgs
    {
        public EntityComponent Entity { get; }
        public int Cost { get; }

        public HiringEventArgs(EntityComponent entity, int cost)
        {
            Entity = entity;
            Cost = cost;
        }
    }
}