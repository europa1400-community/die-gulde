using GuldeLib.Names;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class NamingFactory : Factory<Naming>
    {
        public NamingFactory(GameObject gameObject, GameObject parentObject = null) : base(gameObject, parentObject) { }

        public override GameObject Create(Naming naming)
        {
            // GameObject.name = naming.Name;

            var namingComponent = GameObject.AddComponent<NamingComponent>();
            // namingComponent.FriendlyName = naming.FriendlyName;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}