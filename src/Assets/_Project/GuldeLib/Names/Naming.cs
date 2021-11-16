using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Names
{
    public class Naming : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public string Name { get; set; }

        [Required]
        [OdinSerialize]
        public string FriendlyName { get; set; }
    }
}