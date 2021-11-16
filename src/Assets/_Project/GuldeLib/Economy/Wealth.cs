using System.Collections.Generic;
using GuldeLib.Companies;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Economy
{
    public class Wealth : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public float Money { get; set; }

        [Optional]
        [OdinSerialize]
        public List<Company> Companies { get; } = new List<Company>();

        [Optional]
        [OdinSerialize]
        public Exchange Exchange { get; set; }
    }
}