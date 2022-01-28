using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Production/ProductionAgent", fileName = "production_agent")]
    public class ProductionAgent : TypeObject<ProductionAgent>
    {
        [Required]
        [Setting]
        [OdinSerialize]
        public int ResourceBuffer { get; set; } = 1;

        public override bool SupportsNaming => false;
    }
}