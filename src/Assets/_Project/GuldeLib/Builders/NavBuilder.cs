using System.Collections.Generic;
using GuldeLib.Pathfinding;
using UnityEngine;

namespace GuldeLib.Builders
{
    public class NavBuilder : Builder<Nav>
    {
        public NavBuilder WithNavMap(List<Vector3Int> navMap)
        {
            Object.NavMap = navMap;
            return this;
        }
    }
}