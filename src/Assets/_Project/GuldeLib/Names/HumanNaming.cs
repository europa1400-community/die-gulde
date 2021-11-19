

using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Names
{
    public class HumanNaming : Naming
    {
        [ReadOnly]
        public override GeneratableName Name => $"{FirstName.Value.ToLower()}_{LastName.Value.ToLower()}";

        [ReadOnly]
        public override GeneratableName FriendlyName => $"{FirstName.Value} {LastName.Value}";

        [Generatable]
        [OdinSerialize]
        public GeneratableName FirstName { get; set; } = new GeneratableFemaleHumanLastName();

        [Generatable]
        [OdinSerialize]
        public GeneratableName LastName { get; set; } = new GeneratableHumanLastName();
    }
}