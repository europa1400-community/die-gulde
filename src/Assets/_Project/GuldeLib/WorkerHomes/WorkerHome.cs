using GuldeLib.Generators;
using GuldeLib.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.WorkerHomes
{
    [CreateAssetMenu(menuName = "WorkerHomes/WorkerHome")]
    public class WorkerHome : TypeObject<WorkerHome>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableLocation Location { get; set; } = new GeneratableLocation();
    }
}