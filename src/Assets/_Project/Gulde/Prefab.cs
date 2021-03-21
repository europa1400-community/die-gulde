using UnityEngine;

namespace Gulde
{
    public static class Prefab
    {
        public static GameObject BuildingPrefab => Resources.Load<GameObject>("prefabs/building");
    }
}