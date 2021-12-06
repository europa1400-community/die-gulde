using GuldeLib.Entities;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Pathfinding
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
    }
}