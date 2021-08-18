using Gulde.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gulde.Entities
{
    public class EntityComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public LocationComponent Location { get; set; }

        [OdinSerialize]
        public MapComponent Map { get; set; }

        [OdinSerialize]
        [ReadOnly]
        public EntityRendererComponent Renderer { get; set; }

        void Awake()
        {
            Renderer = GetComponent<EntityRendererComponent>();
        }
    }
}