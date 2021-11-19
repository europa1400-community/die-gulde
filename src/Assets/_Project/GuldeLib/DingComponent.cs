using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib
{
    public class DingComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [InlineEditor]
        public Ding Ding { get; set; }
    }
}