using UnityEngine;

namespace Gulde.Extensions
{
    public static class Vector3Extensions
    {

        public static Vector3Int ToCell(this Vector3 vector) =>
            new Vector3Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));

    }
}