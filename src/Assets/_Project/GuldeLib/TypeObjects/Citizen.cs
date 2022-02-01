using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(fileName = "citizen", menuName = "Society/Citizen")]
    public class Citizen : TypeObject<Citizen>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableWealth Wealth { get; set; } = new GeneratableWealth();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableActionPoint ActionPoint { get; set; } = new GeneratableActionPoint();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableFavor Favor { get; set; } = new GeneratableFavor();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableTalent Talent { get; set; } = new GeneratableTalent();

        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableProfession Profession { get; set; }

        public override bool SupportsNaming => true;
    }
}