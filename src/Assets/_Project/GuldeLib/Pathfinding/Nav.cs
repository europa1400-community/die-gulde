using System.Collections.Generic;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Pathfinding
{
    [CreateAssetMenu(menuName = "Pathfinding/Nav")]
    public class Nav : TypeObject<Nav>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public List<GeneratableVector2Int> NavMap { get; set; } = new List<GeneratableVector2Int>();
    }
}