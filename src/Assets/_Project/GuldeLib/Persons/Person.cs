using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Persons
{
    public class Person : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public Naming Naming { get; set; }
    }
}