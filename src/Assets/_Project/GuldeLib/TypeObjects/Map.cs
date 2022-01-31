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
        public GeneratableMarket Market { get; set; } = new GeneratableMarket();

        [Optional]
        [Generatables]
        [OdinSerialize]
        public List<GeneratableWorkerHome> WorkerHomes { get; set; } = new List<GeneratableWorkerHome>();

        [Optional]
        [Generatables]
        [OdinSerialize]
        public List<GeneratableCompany> Companies { get; set; } = new List<GeneratableCompany>();

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
        public List<MapLayout.BuildSpace> BuildSpaces { get; set; } = new List<MapLayout.BuildSpace>();

        [Required]
        [Setting]
        [OdinSerialize]
        public List<MapLayout.BuildSpace.BuildSpaceType> BuildSpacePriorities { get; set; } = new List<MapLayout.BuildSpace.BuildSpaceType>();

        [Required]
        [Setting]
        [OdinSerialize]
        public int Spacing { get; set; }

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableMapLayout MapLayout { get; set; }

        public override bool SupportsNaming => true;
    }
}