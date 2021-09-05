using System.Collections.Generic;
using GuldeLib.Maps;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Pathfinding
{
    [RequireComponent(typeof(MapComponent))]
    public class NavComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public List<Vector3Int> NavMap { get; } = new List<Vector3Int>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        MapComponent Map { get; set; }

        void Awake()
        {
            this.Log("Nav initializing");

            Map = GetComponent<MapComponent>();

            Map.SizeChanged += OnSizeChanged;
        }

        void OnSizeChanged(object sender, CellEventArgs e)
        {
            this.Log($"Nav recalculating nav map for size {new Vector2Int(e.Cell.x, e.Cell.y)}");

            var size = e.Cell;

            NavMap.Clear();

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