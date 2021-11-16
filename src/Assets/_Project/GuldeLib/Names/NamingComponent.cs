using Sirenix.OdinInspector;

namespace GuldeLib.Names
{
    public class NamingComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        public string FriendlyName { get; set; }
    }
}