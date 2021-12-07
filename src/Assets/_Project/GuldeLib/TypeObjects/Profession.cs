using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(fileName = "profession", menuName = "Players/Professions/Profession")]
    public class Profession : TypeObject<Profession>
    {
        [Required]
        [Setting]
        [OdinSerialize]
        public string Name { get; set; }

        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableInt Level { get; set; }

        public override bool SupportsNaming => false;
    }
}