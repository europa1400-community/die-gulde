using GuldeLib.Persons;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class PersonFactory : Factory<Person, PersonComponent>
    {
        public PersonFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override PersonComponent Create(Person person)
        {
            if (person.Naming?.Value)
            {
                var namingFactory = new NamingFactory(GameObject);
                namingFactory.Create(person.Naming.Value);
            }

            return Component;
        }
    }
}