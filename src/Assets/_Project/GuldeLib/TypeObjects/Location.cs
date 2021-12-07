using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Maps/Location")]
    public class Location : TypeObject<Location>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableVector2Int EntryCell { get; set; } = new GeneratableVector2Int();

        [Optional]
        [Setting]
        [OdinSerialize]
        public GameObject MapPrefab { get; set; }

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableEntityRegistry EntityRegistry { get; set; } = new GeneratableEntityRegistry();

        public override bool SupportsNaming => true;
    }
}