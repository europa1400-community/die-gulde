using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Players
{
    public class Action : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        [MinValue(1)]
        public int PointsPerRound { get; set; }

        [Required]
        [OdinSerialize]
        [MinValue(0)]
        public int Points { get; set; }
    }
}