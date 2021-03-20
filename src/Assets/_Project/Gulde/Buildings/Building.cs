using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gulde.Buildings
{
    [CreateAssetMenu(menuName = "Building")]
    public class Building : ScriptableObject
    {
        public List<Vector3Int> _cellPositions;
    }
}