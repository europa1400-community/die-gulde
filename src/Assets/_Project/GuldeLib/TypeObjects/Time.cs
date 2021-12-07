using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(fileName = "time", menuName = "Time")]
    public class Time : TypeObject<Time>
    {
        /// <summary>
        /// Gets or sets the normal time speed.
        /// </summary>
        [SuffixLabel("min / s")]
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableFloat NormalTimeSpeed { get; set; } = 60;

        [MinValue(0)]
        [Required]
        [Setting]
        [OdinSerialize]
        public int MinYear { get; set; } = 1400;

        [MinValue(0)]
        [MaxValue(nameof(MorningHour))]
        [Required]
        [Setting]
        [OdinSerialize]
        public int MinHour { get; set; } = 6;

        [MinValue(nameof(MinHour))]
        [MaxValue(nameof(EveningHour))]
        [Required]
        [Setting]
        [OdinSerialize]
        public int MorningHour { get; set; } = 8;

        [MinValue(nameof(MorningHour))]
        [MaxValue(nameof(MaxHour))]
        [Required]
        [Setting]
        [OdinSerialize]
        public int EveningHour { get; set; } = 20;

        [MinValue(0)]
        [MaxValue(23)]
        [Required]
        [Setting]
        [OdinSerialize]
        public int MaxHour { get; set; } = 23;

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableInt Minute { get; set; }

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableInt Hour { get; set; }

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableInt Year { get; set; }

        [SuffixLabel("min / s")]
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableFloat TimeSpeed { get; set; } = 5f;

        [ReadOnly]
        [Required]
        [Setting]
        [OdinSerialize]
        public bool AutoAdvance { get; set; }

        public override bool SupportsNaming => false;
    }
}