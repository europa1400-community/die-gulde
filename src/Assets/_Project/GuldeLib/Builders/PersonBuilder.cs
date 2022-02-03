using GuldeLib.Generators;
using GuldeLib.Names;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class PersonBuilder : Builder<Person>
    {
        public PersonBuilder WithNaming(GeneratableHumanNaming naming)
        {
            Object.Naming = naming;
            return this;
        }
    }
}