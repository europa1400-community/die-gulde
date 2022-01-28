using UnityEngine;

namespace GuldeLib.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2Int ToCell(this Vector2 value) => new Vector2Int((int) value.x, (int) value.y);
    }
}