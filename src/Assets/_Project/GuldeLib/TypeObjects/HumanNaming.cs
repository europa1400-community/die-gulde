using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Names/HumanNaming")]
    public class HumanNaming : Naming
    {
        [ReadOnly]
        public override GeneratableName Name => new GeneratableName
            {Value = $"{FirstName?.Value?.ToLower()}_{LastName?.Value?.ToLower()}"};

        [ReadOnly]
        public override GeneratableName FriendlyName => new GeneratableName
            {Value = $"{FirstName?.Value} {LastName?.Value}"};

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableName FirstName { get; set; } = new GeneratableFemaleHumanFirstName();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableName LastName { get; set; } = new GeneratableHumanLastName();
    }
}