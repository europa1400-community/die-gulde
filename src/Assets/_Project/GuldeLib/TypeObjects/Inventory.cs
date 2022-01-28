using System.Collections.Generic;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Inventories/Inventory")]
    public class Inventory : TypeObject<Inventory>
    {
        [Required]
        [Setting]
        [OdinSerialize]
        public int Slots { get; set; } = int.MaxValue;

        [Required]
        [Setting]
        [OdinSerialize]
        public bool UnregisterWhenEmpty { get; set; } = true;

        [Required]
        [Setting]
        [OdinSerialize]
        public bool DisallowUnregister { get; set; }

        [Optional]
        [Setting]
        [OdinSerialize]
        public Dictionary<Item, int> Items { get; set; } = new Dictionary<Item, int>();

        public override bool SupportsNaming => false;
    }
}