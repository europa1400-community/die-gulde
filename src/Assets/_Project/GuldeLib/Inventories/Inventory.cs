using System.Collections.Generic;
using GuldeLib.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Inventories
{
    public class Inventory : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public int Slots { get; set; } = int.MaxValue;

        [Required]
        [OdinSerialize]
        public bool UnregisterWhenEmpty { get; set; }

        [Required]
        [OdinSerialize]
        public bool DisallowUnregister { get; set; }

        [Optional]
        [OdinSerialize]
        public Dictionary<Item, int> Items { get; set; } = new Dictionary<Item, int>();
    }
}