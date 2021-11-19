using GuldeLib.Factories;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Names
{
    public class Naming : TypeObject<Naming>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public virtual GeneratableName Name { get; set; } = new GeneratableName();

        [Required]
        [Generatable]
        [OdinSerialize]
        public virtual GeneratableName FriendlyName { get; set; } = new GeneratableName();

        public override bool HasNaming => false;
    }
}