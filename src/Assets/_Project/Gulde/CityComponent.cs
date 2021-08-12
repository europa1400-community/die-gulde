using System.Collections.Generic;
using Gulde.Buildings;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gulde
{
    public class CityComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        List<Tilemap> TraversableMaps { get; } = new List<Tilemap>();

        [OdinSerialize]
        List<Tilemap> UntraversableMaps { get; } = new List<Tilemap>();

        public List<Vector3Int> NavMap { get; } = new List<Vector3Int>();

        void Awake()
        {
            Locator.CityComponent = this;

            GetCells();
        }

        void GetCells()
        {
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