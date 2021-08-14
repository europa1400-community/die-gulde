using System;

namespace Gulde.Entities
{
    public class EntityEventArgs : EventArgs
    {
        public EntityComponent Entity { get; }

        public EntityEventArgs(EntityComponent entity) => Entity = entity;
    }
}