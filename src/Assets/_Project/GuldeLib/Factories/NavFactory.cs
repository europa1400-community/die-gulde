using System.Linq;
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

            if (!nav || nav.NavMap == null) return GameObject;
            navComponent.NavMap = Enumerable.Cast<Vector2Int>(nav.NavMap).ToList(); //TODO ???

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}