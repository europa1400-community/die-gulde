using GuldeLib.Names;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class NamingFactory : Factory<Naming, NamingComponent>
    {
        public NamingFactory(Naming naming, GameObject gameObject, GameObject parentObject = null) : base(naming, gameObject, parentObject) { }

        public override NamingComponent Create()
        {
            GameObject.name = TypeObject.Name.Value;

            Component.FriendlyName = TypeObject.FriendlyName.Value;

            return Component;
        }
    }
}