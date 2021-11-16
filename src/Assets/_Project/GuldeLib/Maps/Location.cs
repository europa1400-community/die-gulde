using GuldeLib.Entities;
using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Maps
{
    public class Location : SerializedScriptableObject
    {
        [Optional]
        [OdinSerialize]
        public Naming Naming { get; set; }

        [Required]
        [OdinSerialize]
        public Vector3Int EntryCell { get; set; }

        [Required]
        [OdinSerialize]
        public GameObject MapPrefab { get; set; }

        [Required]
        [OdinSerialize]
        public EntityRegistry EntityRegistry { get; set; }
    }
}