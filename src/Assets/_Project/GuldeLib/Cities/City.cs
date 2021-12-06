using System.Collections.Generic;
using GuldeLib.Economy;
using GuldeLib.Generators;
using GuldeLib.Maps;
using GuldeLib.WorkerHomes;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Time = GuldeLib.Timing.Time;

namespace GuldeLib.Cities
{
    [CreateAssetMenu(menuName = "Cities/City")]
    public class City : TypeObject<City>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableMap Map { get; set; } = new GeneratableMap();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableTime Time { get; set; } = new GeneratableTime();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableMarket Market { get; set; } = new GeneratableMarket();

        [Optional]
        [Generatable]
        [OdinSerialize]
        public List<GeneratableWorkerHome> WorkerHomes { get; set; } = new List<GeneratableWorkerHome>();

        [Optional]
        [Generatable]
        [OdinSerialize]
        public List<GeneratableCompany> Companies { get; set; } = new List<GeneratableCompany>();
    }
}