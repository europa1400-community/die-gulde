using System.Collections.Generic;
using GuldeLib.Generators;
using GuldeLib.TypeObjects;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Pathfinding
{
    [CreateAssetMenu(menuName = "Pathfinding/Nav")]
    public class Nav : TypeObject<Nav>
    {
        [Required]
        [Generatables]
        [OdinSerialize]
        public List<GeneratableVector2Int> NavMap { get; set; } = new List<GeneratableVector2Int>();

        public override bool SupportsNaming => false;
    }
}