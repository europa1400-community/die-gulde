using UnityEngine;

namespace Gulde.Extensions
{
    public static class Vector3IntExtension
    {
        public static Vector3 ToWorld(this Vector3Int vector) =>
            new Vector3(vector.x + 0.5f, vector.y + 0.5f, 0f);

        public static float DistanceTo(this Vector3Int from, Vector3Int to) =>
            (to - from).magnitude;

        public static bool IsInBounds(this Vector3Int cell, Vector2Int bounds) =>
            cell.x >= -bounds.x / 2 && cell.x < bounds.x / 2 &&
            cell.y >= -bounds.y / 2 && cell.y < bounds.y / 2;

        public static bool IsInBounds(this Vector3Int cell, Vector3Int bounds) =>
            cell.IsInBounds((Vector2Int)bounds);
    }
}