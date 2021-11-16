using GuldeLib.Pathfinding;
using GuldeLib.Persons;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Companies.Employees
{
    public class Employee : SerializedScriptableObject
    {
        [Required]
        [OdinSerialize]
        public Person Person { get; set; }

        [Required]
        [OdinSerialize]
        public Pathfinder Pathfinder { get; set; }
    }
}