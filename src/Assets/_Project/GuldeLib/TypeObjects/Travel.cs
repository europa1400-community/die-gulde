using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Entities/Travel")]
    public class Travel : TypeObject<Travel>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratablePathfinder Pathfinder { get; set; } = new GeneratablePathfinder();

        public override bool SupportsNaming => false;
    }
}