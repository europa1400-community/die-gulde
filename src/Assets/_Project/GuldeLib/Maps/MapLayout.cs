using System;
using System.Collections.Generic;
using GuldeLib.Generators;
using GuldeLib.TypeObjects;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Maps
{
    [Serializable]
    public class MapLayout
    {
        [Required]
        [OdinSerialize]
        public Dictionary<Vector2Int, BuildSpace> CellToBuildSpace { get; set; } =
            new Dictionary<Vector2Int, BuildSpace>();

        [Required]
        [OdinSerialize]
        public Dictionary<Vector2Int, (Building, Vector2Int)> CellToBuilding { get; set; } =
            new Dictionary<Vector2Int, (Building, Vector2Int)>();

        public Vector2Int? PlaceBuilding(Building building)
        {
            foreach (var pair in CellToBuildSpace)
            {
                var buildSpaceCell = pair.Key;
                var buildSpace = pair.Value;

                if (!IsPlacable(building, buildSpace, buildSpaceCell)) continue;

                var buildingCenterCell = new Vector2Int(building.Size.x / 2, building.Size.y / 2);
                var buildingEntryCell = buildSpaceCell - (buildingCenterCell - building.EntryCell);

                CellToBuilding[buildSpaceCell] = (building, buildingEntryCell);

                return buildingEntryCell;
            }

            return null;
        }

        public bool IsPlacable(Building building, BuildSpace buildSpace, Vector2Int cell)
        {
            if (buildSpace.Size.x < building.Size.x ||
                buildSpace.Size.y < building.Size.y) return false;

            if (CellToBuilding.ContainsKey(cell)) return false;

            if (building.BuildSpaceType != buildSpace.Type) return false;

            return true;
        }

        [Serializable]
        public class BuildSpace
        {
            [OdinSerialize]
            public Vector2Int Size { get; set; }

            [OdinSerialize]
            public int Count { get; set; }

            [OdinSerialize]
            public BuildSpaceType Type { get; set; }

            public enum BuildSpaceType
            {
                Market,
                CityHall,
                Church,
                Company,
                Residence,
                WorkerHome,
                Mine,
                TreeFarm,
                Decorational,
            }
        }
    }
}