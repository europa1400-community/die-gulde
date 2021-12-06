using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Players
{
    [CreateAssetMenu(menuName = "Players/Action")]
    public class Action : TypeObject<Action>
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