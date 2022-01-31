using GuldeLib.Companies.Carts;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Companies/Carts/Cart")]
    public class Cart : TypeObject<Cart>
    {
        [Required]
        [Setting]
        [OdinSerialize]
        public CartComponent.CartType CartType { get; set; }

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableTravel Travel { get; set; } = new GeneratableTravel();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableExchange Exchange { get; set; } = new GeneratableExchange();

        public override bool SupportsNaming => false;
    }
}