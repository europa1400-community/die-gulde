using UnityEngine;

namespace Gulde.Extensions
{
    public static class Vector2IntExtensions
    {
        public static bool IsInBounds(this Vector2Int cell, Vector2Int bounds) =>
            cell.x >= -bounds.x / 2 && cell.x < bounds.x / 2 &&
            cell.y >= -bounds.y / 2 && cell.y < bounds.y / 2;

        public static bool IsInBounds(this Vector2Int cell, Vector3Int bounds) =>
            cell.IsInBounds((Vector2Int)bounds);
    }
}