using System.Collections.Generic;
using GuldeLib.Generators;
using GuldeLib.Inventories;
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
        public int MaxCapacity { get; set; } = int.MaxValue;

        [Required]
        [Setting]
        [OdinSerialize]
        public int MaxSlots { get; set; } = 1;

        [Required]
        [Setting]
        [OdinSerialize]
        public bool AllowAutoUnregister { get; set; } = true;

        [Optional]
        [Setting]
        [OdinSerialize]
        public List<InventoryComponent.Slot> Slots { get; set; } = new List<InventoryComponent.Slot>();

        public override bool SupportsNaming => false;
    }
}