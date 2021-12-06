using GuldeLib.Generators;
using GuldeLib.Names;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Persons
{
    [CreateAssetMenu(menuName = "Persons/Person")]
    public class Person : TypeObject<Person>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableHumanNaming Naming { get; set; } = new GeneratableHumanNaming();
    }
}