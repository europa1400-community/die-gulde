using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Maps/Location")]
    public class Location : TypeObject<Location>
    {
        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableBuilding Building { get; set; }

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