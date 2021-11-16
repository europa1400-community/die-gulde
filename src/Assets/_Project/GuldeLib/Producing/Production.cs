using GuldeLib.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Producing
{
    public class Production : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public Exchange Exchange { get; set; }

        [Required]
        [OdinSerialize]
        public Assignment Assignment { get; set; }

        [Required]
        [OdinSerialize]
        public ProductionRegistry ProductionRegistry { get; set; }
    }
}