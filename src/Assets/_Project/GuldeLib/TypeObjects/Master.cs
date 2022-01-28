using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Companies/Master")]
    public class Master : TypeObject<Master>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableProductionAgent ProductionAgent { get; set; }

        public override bool SupportsNaming => false;
    }
}