using UnityEngine;

namespace Gulde.Extensions
{
    public static class Vector3IntExtension
    {
        public static Vector3 ToWorld(this Vector3Int vector) =>
            new Vector3(vector.x + 0.5f, vector.y + 0.5f, 0f);
    }
}