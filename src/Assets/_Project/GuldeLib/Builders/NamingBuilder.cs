using GuldeLib.Names;

namespace GuldeLib.Builders
{
    public class NamingBuilder : Builder<Naming>
    {
        public NamingBuilder WithName(string name)
        {
            // Object.Name = name;
            return this;
        }

        public NamingBuilder WithFriendlyName(string friendlyName)
        {
            // Object.FriendlyName = friendlyName;
            return this;
        }
    }
}