using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.Names;
using GuldeLib.Pathfinding;
using UnityEngine;

namespace GuldeLib.Builders
{
    public class MapBuilder : Builder<Map>
    {
        public MapBuilder WithSize(Vector2Int size)
        {
            Object.Size = size;
            return this;
        }

        public MapBuilder WithNaming(Naming naming)
        {
            Object.Naming = naming;
            return this;
        }

        public MapBuilder WithNav(Nav nav)
        {
            Object.Nav = nav;
            return this;
        }

        public MapBuilder WithEntityRegistry(EntityRegistry entityRegistry)
        {
            Object.EntityRegistry = entityRegistry;
            return this;
        }
    }
}