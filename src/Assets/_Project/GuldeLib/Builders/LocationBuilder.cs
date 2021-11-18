using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.Names;
using UnityEngine;

namespace GuldeLib.Builders
{
    public class LocationBuilder : Builder<Location>
    {
        public LocationBuilder WithNaming(Naming naming)
        {
            Object.Naming = naming;
            return this;
        }

        public LocationBuilder WithEntryCell(Vector3Int entryCell)
        {
            Object.EntryCell = entryCell;
            return this;
        }

        public LocationBuilder WithEntryCell(int x, int y)
        {
            Object.EntryCell = new Vector3Int(x, y, 0);
            return this;
        }

        public LocationBuilder WithMapPrefab(GameObject mapPrefab)
        {
            Object.MapPrefab = mapPrefab;
            return this;
        }

        public LocationBuilder WithEntityRegistry(EntityRegistry entityRegistry)
        {
            Object.EntityRegistry = entityRegistry;
            return this;
        }
    }
}