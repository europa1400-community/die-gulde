using System.Collections.Generic;
using GuldeLib.Generators;
using GuldeLib.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Maps/Map")]
    public class Map : TypeObject<Map>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableVector2Int Size { get; set; } = new GeneratableVector2Int();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableNav Nav { get; set; } = new GeneratableNav();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableEntityRegistry EntityRegistry { get; set; } = new GeneratableEntityRegistry();

        [Required]
        [Setting]
        [OdinSerialize]
        public List<BuildSpace> BuildSpaces { get; set; } = new List<BuildSpace>();

        [Required]
        [Setting]
        [OdinSerialize]
        public List<BuildSpaceType> BuildSpacePriorities { get; set; } = new List<BuildSpaceType>();

        [Required]
        [Setting]
        [OdinSerialize]
        public int Spacing { get; set; }

        [Required]
        [Setting]
        [OdinSerialize]
        public GeneratableMapLayout MapLayout { get; set; }

        public override bool SupportsNaming => true;
    }
}