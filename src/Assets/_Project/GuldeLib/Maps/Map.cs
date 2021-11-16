using GuldeLib.Entities;
using GuldeLib.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Maps
{
    public class Map : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public Vector2Int Size { get; set; }

        [Required]
        [OdinSerialize]
        public Nav Nav { get; set; }

        [Required]
        [OdinSerialize]
        public EntityRegistry EntityRegistry { get; set; }
    }
}