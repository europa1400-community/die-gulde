using GuldeLib.Pathfinding;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class NavFactory : Factory<Nav>
    {
        public NavFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Nav nav)
        {
            var navComponent = GameObject.AddComponent<NavComponent>();

            navComponent.NavMap = nav.NavMap;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}