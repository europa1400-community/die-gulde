using GuldeLib.Names;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class NamingFactory : Factory<Naming, NamingComponent>
    {
        public NamingFactory(GameObject gameObject, GameObject parentObject = null) : base(gameObject, parentObject) { }

        public override NamingComponent Create(Naming naming)
        {
            GameObject.name = naming.Name.Value;

            Component.FriendlyName = naming.FriendlyName.Value;

            return Component;
        }
    }
}