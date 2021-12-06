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
        MapComponent Map => this.GetCachedComponent<MapComponent>();

        void Awake()
        {
            this.Log("Nav initializing");
        }

        void Start()
        {
            if (Map) Map.SizeChanged += OnSizeChanged;
        }

        void OnSizeChanged(object sender, CellEventArgs e)
        {
            this.Log($"Nav recalculating nav map for size {e.Cell.ToString()}");

            var size = e.Cell;

            NavMap.Clear();

            for (var x = -size.x / 2; x < size.x / 2; x++)
            {
                for (var y = -size.y / 2; y < size.y / 2; y++)
                {
                    var cell = new Vector2Int(x, y);
                    NavMap.Add(cell);
                }
            }
        }
    }
}