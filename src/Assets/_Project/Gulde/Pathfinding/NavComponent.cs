using System.Collections.Generic;
using Gulde.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gulde.Pathfinding
{
    [RequireComponent(typeof(MapComponent))]
    public class NavComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Info")]
        public List<Vector3Int> NavMap { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        MapComponent Map { get; set; }

        void Awake()
        {
            Map = GetComponent<MapComponent>();

            Map.SizeChanged += OnSizeChanged;
        }

        void OnSizeChanged(object sender, CellEventArgs e)
        {
            var size = e.Cell;

            for (var x = -size.x / 2; x < size.x / 2; x++)
            {
                for (var y = -size.y / 2; y < size.y / 2; y++)
                {
                    var cell = new Vector3Int(x, y, 0);
                    NavMap.Add(cell);
                }
            }
        }
    }
}