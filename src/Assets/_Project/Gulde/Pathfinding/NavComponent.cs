using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gulde.Pathfinding
{
    public class NavComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        List<Tilemap> TraversableMaps { get; set; }

        [OdinSerialize]
        List<Tilemap> UntraversableMaps { get; set; }

        [ShowInInspector]
        [InlineButton("GetCells", "Refresh")]
        public List<Vector3Int> NavMap { get; set; }

        void Awake()
        {
            GetCells();
        }

        void GetCells()
        {
            NavMap = new List<Vector3Int>();

            foreach (var tilemap in TraversableMaps)
            {
                foreach (var cell in tilemap.cellBounds.allPositionsWithin)
                {
                    if (!tilemap.HasTile(cell)) continue;
                    NavMap.Add(cell);
                }
            }

            foreach (var tilemap in UntraversableMaps)
            {
                foreach (var cell in tilemap.cellBounds.allPositionsWithin)
                {
                    if (!tilemap.HasTile(cell)) continue;
                    NavMap.Remove(cell);
                }
            }
        }
    }
}