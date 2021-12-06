using GuldeLib.Persons;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class PersonFactory : Factory<Person>
    {
        public PersonFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Person person)
        {
            var namingFactory = new NamingFactory(GameObject);
            namingFactory.Create(person.Naming.Value);

            var personComponent = GameObject.AddComponent<PersonComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}