namespace Gulde.Entities
{
    public class EntityEventArgs
    {
        public EntityComponent Entity { get; }

        public EntityEventArgs(EntityComponent entity) => Entity = entity;
    }
}