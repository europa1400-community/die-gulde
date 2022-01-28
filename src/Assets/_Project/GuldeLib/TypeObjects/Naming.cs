using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Names/Naming")]
    public class Naming : TypeObject<Naming>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public virtual GeneratableName Name { get; set; } = new GeneratableName();

        [Required]
        [Generatable]
        [OdinSerialize]
        public virtual GeneratableName FriendlyName { get; set; } = new GeneratableName();

        public override bool SupportsNaming => false;
    }
}