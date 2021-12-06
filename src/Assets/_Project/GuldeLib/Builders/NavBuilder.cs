using System.Collections.Generic;
using GuldeLib.Generators;
using GuldeLib.Pathfinding;
using UnityEngine;

namespace GuldeLib.Builders
{
    public class NavBuilder : Builder<Nav>
    {
        public NavBuilder WithNavMap(List<GeneratableVector2Int> navMap)
        {
            Object.NavMap = navMap;
            return this;
        }
    }
}