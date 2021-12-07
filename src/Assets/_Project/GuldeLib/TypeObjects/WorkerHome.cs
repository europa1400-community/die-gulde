using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "WorkerHomes/WorkerHome")]
    public class WorkerHome : TypeObject<WorkerHome>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableLocation Location { get; set; } = new GeneratableLocation();

        public override bool SupportsNaming => false;
    }
}