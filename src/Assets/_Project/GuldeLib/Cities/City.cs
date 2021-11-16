using Codice.Client.BaseCommands;
using GuldeLib.Timing;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Cities
{
    public class City : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public Map Map { get; set; }

        [Required]
        [OdinSerialize]
        public Time Time { get; set; }
    }
}