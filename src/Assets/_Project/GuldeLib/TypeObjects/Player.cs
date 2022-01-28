using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Players/Player")]
    public class Player : TypeObject<Player>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableWealth Wealth { get; set; } = new GeneratableWealth();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableAction Action { get; set; } = new GeneratableAction();

        [Optional]
        [Generatable]
        [OdinSerialize]
        public GeneratableProfession Profession { get; set; }

        public override bool SupportsNaming => true;
    }
}