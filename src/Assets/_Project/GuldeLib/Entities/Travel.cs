using GuldeLib.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Entities
{
    public class Travel : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public Pathfinder Pathfinder { get; set; }
    }
}