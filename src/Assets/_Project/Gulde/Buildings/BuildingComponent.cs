using UnityEngine;

namespace Gulde.Buildings
{
    public class BuildingComponent : MonoBehaviour
    {
        [SerializeField] public BuildingLayout _buildingLayout;

        public Orientation Orientation { get; set; }
        public Vector3Int Position { get; set; }
    }
}