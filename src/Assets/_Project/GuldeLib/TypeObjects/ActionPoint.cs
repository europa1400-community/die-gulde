using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(fileName = "action_point", menuName = "Society/ActionPoint")]
    public class ActionPoint : TypeObject<ActionPoint>
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