

using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Names
{
    [CreateAssetMenu(menuName = "Names/HumanNaming")]
    public class HumanNaming : Naming
    {
        [ReadOnly]
        public override GeneratableName Name => $"{FirstName?.Value?.ToLower()}_{LastName?.Value?.ToLower()}";

        [ReadOnly]
        public override GeneratableName FriendlyName => $"{FirstName?.Value} {LastName?.Value}";

        [Generatable]
        [OdinSerialize]
        public GeneratableName FirstName { get; set; } = new GeneratableFemaleHumanFirstName();

        [Generatable]
        [OdinSerialize]
        public GeneratableName LastName { get; set; } = new GeneratableHumanLastName();
    }
}