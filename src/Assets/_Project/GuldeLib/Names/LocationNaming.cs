using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Names
{
    public class LocationNaming : Naming
    {
        [ReadOnly]
        public override GeneratableName Name =>
            $"location_{LocationName?.Value?.ToLower().Replace(' ', '_')}";

        [ReadOnly]
        public override GeneratableName FriendlyName => LocationName?.Value;

        [Required]
        [OdinSerialize]
        [Generatable]
        public GeneratableLocationName LocationName { get; set; }
    }
}