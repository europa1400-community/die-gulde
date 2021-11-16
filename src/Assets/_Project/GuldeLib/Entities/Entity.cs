using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Entities
{
    public class Entity : SerializedScriptableObject
    {
        [Optional]
        [OdinSerialize]
        public Naming Naming { get; set; }
    }
}