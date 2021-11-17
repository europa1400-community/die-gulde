using GuldeLib.Entities;
using GuldeLib.Names;

namespace GuldeLib.Builders
{
    public class EntityBuilder : Builder<Entity>
    {
        public EntityBuilder WithNaming(Naming naming)
        {
            Object.Naming = naming;
            return this;
        }
    }
}