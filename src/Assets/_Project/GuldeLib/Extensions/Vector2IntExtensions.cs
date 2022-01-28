using System.Collections.Generic;
using UnityEngine;

namespace GuldeLib.Extensions
{
    public static class Vector2IntExtensions
    {
        public static float DistanceTo(this Vector2Int value, Vector2Int to) => (to - value).magnitude;

        public static List<Vector2Int> GetCellsWithin(this Vector2Int value, Vector2Int size)
        {
            var middleX = size.x / 2;
            var middleY = size.y / 2;
            var minX = value.x - middleX;
            var maxX = value.x + size.x - middleX;
            var minY = value.y - middleY;
            var maxY = value.y + size.y - middleY;

            var cells = new List<Vector2Int>();

            for (var x = minX; x < maxX; x++)
            {
                for (var y = minY; y < maxY; y++)
                {
                    var cell = new Vector2Int(x, y);
                    cells.Add(cell);
                }
            }

            return cells;
        }
    }
}