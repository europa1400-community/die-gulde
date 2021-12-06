using System.Collections.Generic;
using GuldeLib.Entities;
using GuldeLib.Generators;
using GuldeLib.Names;
using GuldeLib.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Maps
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
        public GeneratableNaming Naming { get; set; } = new GeneratableNaming();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableNav Nav { get; set; } = new GeneratableNav();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableEntityRegistry EntityRegistry { get; set; } = new GeneratableEntityRegistry();

        [Required]
        [OdinSerialize]
        public List<BuildSpace> BuildSpaces { get; set; } = new List<BuildSpace>();

        [Required]
        [OdinSerialize]
        public List<BuildSpaceType> BuildSpacePriorities { get; set; } = new List<BuildSpaceType>();

        [Required]
        [OdinSerialize]
        public int Spacing { get; set; }

        [Required]
        [OdinSerialize]
        public GeneratableMapLayout MapLayout { get; set; }
    }
}