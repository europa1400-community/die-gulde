using GuldeLib.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Player
{
    public class Player : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public Wealth Wealth { get; set; }

        [Required]
        [OdinSerialize]
        public Action Action { get; set; }
    }
}