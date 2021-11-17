using System.Collections.Generic;
using GuldeLib.Economy;
using GuldeLib.Maps;
using GuldeLib.Timing;
using GuldeLib.WorkerHomes;
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

        [Required]
        [OdinSerialize]
        public Market Market { get; set; }

        [Optional]
        [OdinSerialize]
        public List<WorkerHome> WorkerHomes { get; set; }
    }
}