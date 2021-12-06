using System.Collections.Generic;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib
{
    [CreateAssetMenu(fileName="game", menuName="Game")]
    public class Game : TypeObject<Game>
    {
        [Required]
        [OdinSerialize]
        public string SceneName { get; set; } = "scene_game";

        [Required]
        [Generatable]
        [OdinSerialize]
        public List<GeneratablePlayer> Players { get; set; } = new List<GeneratablePlayer>();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableCity City { get; set; } = new GeneratableCity();
    }
}