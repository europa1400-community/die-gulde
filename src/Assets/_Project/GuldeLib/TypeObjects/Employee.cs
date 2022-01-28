using GuldeLib.Generators;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.TypeObjects
{
    [CreateAssetMenu(menuName = "Companies/Employees/Employee")]
    public class Employee : TypeObject<Employee>
    {
        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratablePerson Person { get; set; } = new GeneratablePerson();

        [Required]
        [Generatable]
        [OdinSerialize]
        public GeneratableTravel Travel { get; set; } = new GeneratableTravel();

        public override bool SupportsNaming => false;
    }
}