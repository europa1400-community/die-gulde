using System.Collections.Generic;
using GuldeLib.Generators;
using GuldeLib.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(fileName = "building", menuName = "Maps/Buildings/Building")]
    public class Building : TypeObject<Building>
    {
        [Required]
        [Setting]
        [OdinSerialize]
        public Vector2Int Size { get; set; }

        [Required]
        [Setting]
        [OdinSerialize]
        public Vector2Int EntryCell { get; set; }

        [Required]
        [Setting]
        [OdinSerialize]
        public BuildSpaceType BuildSpaceType { get; set; }

        public override bool SupportsNaming => false;
    }
}