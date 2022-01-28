using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Players/Action")]
    public class Action : TypeObject<Action>
    {
        [MinValue(1)]
        [Required]
        [Setting]
        [OdinSerialize]
        public int PointsPerRound { get; set; }

        [MinValue(0)]
        [Required]
        [Setting]
        [OdinSerialize]
        public int Points { get; set; }

        public override bool SupportsNaming => false;
    }
}