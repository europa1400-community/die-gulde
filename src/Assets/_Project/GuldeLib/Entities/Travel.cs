using GuldeLib.Generators;
using GuldeLib.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Entities
{
    [CreateAssetMenu(menuName = "Entities/Travel")]
    public class Travel : TypeObject<Travel>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratablePathfinder Pathfinder { get; set; } = new GeneratablePathfinder();
    }
}