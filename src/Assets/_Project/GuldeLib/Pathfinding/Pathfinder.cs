using GuldeLib.Entities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Pathfinding
{
    public class Pathfinder : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public float Speed { get; set; }

        [Required]
        [OdinSerialize]
        public Entity Entity { get; set; }
    }
}