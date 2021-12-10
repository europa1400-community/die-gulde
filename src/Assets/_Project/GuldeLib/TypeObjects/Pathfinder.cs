using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Pathfinding/Pathfinder")]
    public class Pathfinder : TypeObject<Pathfinder>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableFloat Speed { get; set; } = new GeneratableDeviationFloat();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableEntity Entity { get; set; } = new GeneratableEntity();

        public override bool SupportsNaming => false;
    }
}