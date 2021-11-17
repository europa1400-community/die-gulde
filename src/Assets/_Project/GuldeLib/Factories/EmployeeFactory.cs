using GuldeLib.Companies.Employees;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class EmployeeFactory : Factory<Employee>
    {
        public EmployeeFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Employee employee)
        {
            var pathfinderFactory = new PathfinderFactory(GameObject);
            pathfinderFactory.Create(employee.Pathfinder);

            var personFactory = new PersonFactory(GameObject);
            personFactory.Create(employee.Person);

            var employeeComponent = GameObject.AddComponent<EmployeeComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}