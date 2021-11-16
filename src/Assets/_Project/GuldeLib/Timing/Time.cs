using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Timing
{
    public class Time : SerializedScriptableObject
    {
        /// <summary>
        /// Gets or sets the normal time speed.
        /// </summary>
        [Required]
        [OdinSerialize]
        [BoxGroup("Settings")]
        [SuffixLabel("min / s")]
        [MinValue(1)]
        public float NormalTimeSpeed { get; set; }

        [Required]
        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue(0)]
        public int MinYear { get; set; }

        [Required]
        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue(0)]
        [MaxValue("MorningHour")]
        public int MinHour { get; set; }

        [Required]
        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue("MinHour")]
        [MaxValue("EveningHour")]
        public int MorningHour { get; set; }

        [Required]
        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue("MorningHour")]
        [MaxValue("MaxHour")]
        public int EveningHour { get; set; }

        [Required]
        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue(0)]
        [MaxValue(23)]
        public int MaxHour { get; set; }

        [Required]
        [OdinSerialize]
        [BoxGroup("Info")]
        [MinValue(0)]
        [MaxValue(59)]
        public int Minute { get; set; }

        [Required]
        [OdinSerialize]
        [BoxGroup("Info")]
        [MinValue("MinHour")]
        [MaxValue("MaxHour")]
        public int Hour { get; set; }

        [Required]
        [OdinSerialize]
        [BoxGroup("Info")]
        [MinValue("MinYear")]
        public int Year { get; set; }

        [Required]
        [OdinSerialize]
        [BoxGroup("Info")]
        [SuffixLabel("min / s")]
        public float TimeSpeed { get; set; } = 5f;

        [Required]
        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public bool AutoAdvance { get; set; }
    }
}