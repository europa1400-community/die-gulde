using GuldeLib.Economy;
using GuldeLib.Entities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Companies.Carts
{
    public class Cart : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public CartType CartType { get; set; }

        [Required]
        [OdinSerialize]
        public Travel Travel { get; set; }

        [Required]
        [OdinSerialize]
        public Exchange Exchange { get; set; }
    }
}