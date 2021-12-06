using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Players.Professions
{
    [CreateAssetMenu(fileName = "profession", menuName = "Players/Professions/Profession")]
    public class Profession : TypeObject<Profession>
    {
        [Required]
        [OdinSerialize]
        public string Name { get; set; }

        [OdinSerialize]
        [Generatable]
        [Optional]
        public GeneratableInt Level { get; set; }
    }
}