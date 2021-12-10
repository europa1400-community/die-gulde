using System.Collections.Generic;
using System.Linq;
using GuldeLib.Maps;
using GuldeLib.Pathfinding;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class NavFactory : Factory<Nav, NavComponent>
    {
        public NavFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override NavComponent Create(Nav nav)
        {
            Component.CalculateNavMap();

            return Component;
        }
    }
}