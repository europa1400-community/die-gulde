using Gulde.Entities;
using Gulde.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Buildings
{
    public class BuildingComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        public BuildingLayout Layout { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        public Orientation Orientation { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        public Vector3Int Position { get; set; }

    }
}