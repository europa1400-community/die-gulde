using GuldeLib.Society;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class PersonFactory : Factory<Person, PersonComponent>
    {
        public PersonFactory(Person person, GameObject gameObject = null, GameObject parentObject = null) : base(person, gameObject, parentObject)
        {
        }

        public override PersonComponent Create()
        {
            if (TypeObject.Naming?.Value)
            {
                var namingFactory = new NamingFactory(TypeObject.Naming.Value, GameObject);
                namingFactory.Create();
            }

            return Component;
        }
    }
}