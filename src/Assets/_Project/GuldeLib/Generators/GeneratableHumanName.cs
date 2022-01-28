using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Generators
{
    public abstract class GeneratableHumanName : GeneratableName
    {
        [Required]
        [OdinSerialize]
        public NameTable NameTable { get; set; }

        protected override bool IsValid => NameTable;
    }
}