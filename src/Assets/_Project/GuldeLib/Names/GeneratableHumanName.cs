using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Names
{
    public abstract class GeneratableHumanName : GeneratableName
    {
        [Required]
        [OdinSerialize]
        public NameTable NameTable { get; set; }
    }
}