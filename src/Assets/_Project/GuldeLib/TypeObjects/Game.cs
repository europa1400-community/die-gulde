using System.Collections.Generic;
using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(fileName="game", menuName="Game")]
    public class Game : TypeObject<Game>
    {
        [Required]
        [Setting]
        [OdinSerialize]
        public string SceneName { get; set; } = "scene_game";

        [Required]
        [Generatables]
        [OdinSerialize]
        public List<GeneratableCitizen> Players { get; set; } = new List<GeneratableCitizen>();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableCity City { get; set; } = new GeneratableCity();

        public override bool SupportsNaming => true;
    }
}