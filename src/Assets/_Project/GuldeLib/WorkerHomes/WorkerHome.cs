using GuldeLib.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.WorkerHomes
{
    public class WorkerHome : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public Location Location { get; set; }
    }
}