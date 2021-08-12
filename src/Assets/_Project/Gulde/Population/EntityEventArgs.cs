namespace Gulde.Population
{
    public class EntityEventArgs
    {
        public EntityComponent Entity { get; }

        public EntityEventArgs(EntityComponent entity) => Entity = entity;
    }
}