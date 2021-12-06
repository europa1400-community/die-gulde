using GuldeLib.Generators;
using GuldeLib.Pathfinding;
using GuldeLib.Persons;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Companies.Employees
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
        public GeneratablePathfinder Pathfinder { get; set; } = new GeneratablePathfinder();
    }
}