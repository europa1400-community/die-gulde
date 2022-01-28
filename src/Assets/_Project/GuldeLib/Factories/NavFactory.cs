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
        public NavFactory(Nav nav, GameObject gameObject = null, GameObject parentObject = null) : base(nav, gameObject, parentObject)
        {
        }

        public override NavComponent Create()
        {
            Component.CalculateNavMap();

            return Component;
        }
    }
}