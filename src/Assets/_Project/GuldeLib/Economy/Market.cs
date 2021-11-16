using System.Collections.Generic;
using GuldeLib.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Economy
{
    public class Market : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public List<Exchange> Exchanges { get; } = new List<Exchange>();

        [Required]
        [OdinSerialize]
        public Location Location { get; set; }
    }
}