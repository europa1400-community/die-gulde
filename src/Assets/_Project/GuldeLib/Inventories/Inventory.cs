using System.Collections.Generic;
using GuldeLib.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Inventories
{
    [CreateAssetMenu(menuName = "Inventories/Inventory")]
    public class Inventory : TypeObject<Inventory>
    {
        [Required]
        [OdinSerialize]
        public int Slots { get; set; } = int.MaxValue;

        [Required]
        [OdinSerialize]
        public bool UnregisterWhenEmpty { get; set; } = true;

        [Required]
        [OdinSerialize]
        public bool DisallowUnregister { get; set; }

        [Optional]
        [OdinSerialize]
        public Dictionary<Item, int> Items { get; set; } = new Dictionary<Item, int>();
    }
}