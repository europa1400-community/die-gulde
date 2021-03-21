using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gulde.Buildings
{
    [CreateAssetMenu(menuName = "Building")]
    public class BuildingLayout : ScriptableObject
    {
        public List<Vector3Int> _cellPositions;
        public Vector3Int _entrancePosition;
        public BuildingType _buildingType;
    }
}