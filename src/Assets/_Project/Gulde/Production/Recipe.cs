using System.Collections.Generic;
using Gulde.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Production
{
    [HideMonoScript]
    [CreateAssetMenu(menuName="Recipe")]
    public class Recipe : SerializedScriptableObject
    {
        [OdinSerialize]
        [Required]
        public Dictionary<Item, int> Resources { get; set; } = new Dictionary<Item, int>();

        [OdinSerialize]
        [Required]
        public Item Product { get; set; }

        [OdinSerialize]
        [MinValue(0f)]
        public float Time { get; set; }

        [OdinSerialize]
        public InventoryType InventoryType { get; set; }

        [OdinSerialize]
        public bool IsExternal { get; set; }
    }
}