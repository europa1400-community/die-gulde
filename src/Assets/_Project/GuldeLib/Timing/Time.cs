using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Timing
{
    [CreateAssetMenu(fileName = "time", menuName = "Time")]
    public class Time : TypeObject<Time>
    {
        public override bool HasNaming => false;

        /// <summary>
        /// Gets or sets the normal time speed.
        /// </summary>
        [Required]
        [OdinSerialize]
        [Generatable]
        [SuffixLabel("min / s")]
        public GeneratableFloat NormalTimeSpeed { get; set; } = 60;

        [Required]
        [OdinSerialize]
        [MinValue(0)]
        public int MinYear { get; set; } = 1400;

        [Required]
        [OdinSerialize]
        [MinValue(0)]
        [MaxValue("MorningHour")]
        public int MinHour { get; set; } = 6;

        [Required]
        [OdinSerialize]
        [MinValue("MinHour")]
        [MaxValue("EveningHour")]
        public int MorningHour { get; set; } = 8;

        [Required]
        [OdinSerialize]
        [MinValue("MorningHour")]
        [MaxValue("MaxHour")]
        public int EveningHour { get; set; } = 20;

        [Required]
        [OdinSerialize]
        [MinValue(0)]
        [MaxValue(23)]
        public int MaxHour { get; set; } = 23;

        [Required]
        [OdinSerialize]
        [Generatable]
        public GeneratableInt Minute { get; set; }

        [Required]
        [OdinSerialize]
        [Generatable]
        public GeneratableInt Hour { get; set; }

        [Required]
        [OdinSerialize]
        [Generatable]
        public GeneratableInt Year { get; set; }

        [Required]
        [OdinSerialize]
        [Generatable]
        [SuffixLabel("min / s")]
        public GeneratableFloat TimeSpeed { get; set; } = 5f;

        [Required]
        [OdinSerialize]
        [ReadOnly]
        public bool AutoAdvance { get; set; }
    }
}