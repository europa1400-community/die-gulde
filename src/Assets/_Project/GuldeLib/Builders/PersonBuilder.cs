using GuldeLib.Names;
using GuldeLib.Persons;

namespace GuldeLib.Builders
{
    public class PersonBuilder : Builder<Person>
    {
        public PersonBuilder WithNaming(Naming naming)
        {
            Object.Naming = naming;
            return this;
        }
    }
}