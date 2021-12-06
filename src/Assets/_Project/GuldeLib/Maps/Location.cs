using GuldeLib.Entities;
using GuldeLib.Generators;
using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Maps
{
    [CreateAssetMenu(menuName = "Maps/Location")]
    public class Location : TypeObject<Location>
    {
        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableNaming Naming { get; set; }

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableVector2Int EntryCell { get; set; } = new GeneratableVector2Int();

        [Optional]
        [FoldoutGroup("Settings")]
        [OdinSerialize]
        public GameObject MapPrefab { get; set; }

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableEntityRegistry EntityRegistry { get; set; } = new GeneratableEntityRegistry();
    }
}