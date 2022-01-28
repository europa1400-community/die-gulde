using GuldeLib.Generators;
using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.TypeObjects
{
    public class LocationNaming : Naming
    {
        [ReadOnly]
        public override GeneratableName Name => new GeneratableName { Value = LocationNameToName() };

        [ReadOnly]
        public override GeneratableName FriendlyName => new GeneratableName { Value = LocationName?.Value };

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableLocationName LocationName { get; set; }

        string LocationNameToName() =>
            LocationName?.Value == null ? "" : $"location_{LocationName?.Value?.ToLower().Replace(' ', '_')}";
    }
}