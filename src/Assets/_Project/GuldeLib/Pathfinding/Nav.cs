using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Pathfinding
{
    public class Nav : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public List<Vector3Int> NavMap { get; } = new List<Vector3Int>();
    }
}