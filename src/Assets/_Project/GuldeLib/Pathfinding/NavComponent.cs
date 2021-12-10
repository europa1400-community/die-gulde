using System.Collections.Generic;
using GuldeLib.Maps;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Pathfinding
{
    public class NavComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public List<Vector2Int> NavMap { get; set; } = new List<Vector2Int>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        MapComponent MapComponent => GetComponent<MapComponent>();

        void Awake()
        {
            this.Log("Nav initializing");
        }

        public void CalculateNavMap()
        {
            this.Log($"Calculating nav map.");

            NavMap = new List<Vector2Int>();

            for (var x = -MapComponent.Size.x / 2; x < MapComponent.Size.x / 2; x++)
            {
                for (var y = -MapComponent.Size.y / 2; y < MapComponent.Size.y / 2; y++)
                {
                    var cell = new Vector2Int(x, y);
                    NavMap.Add(cell);
                }
            }

            foreach (var pair in MapComponent.MapLayout.CellToBuilding)
            {
                var cell = pair.Key;
                var buildingPair = pair.Value;
                var building = buildingPair.Item1;
                var entryCell = buildingPair.Item2;

                for (var x = 0; x < building.Size.x; x++)
                {
                    for (var y = 0; y < building.Size.y; y++)
                    {
                        var buildingCell = new Vector2Int(x, y);
                        var worldCell = entryCell - building.EntryCell + buildingCell;

                        if (worldCell == entryCell) continue;

                        if (NavMap.Contains(buildingCell)) NavMap.Remove(worldCell);
                    }
                }
            }
        }
    }
}