using GuldeLib.Companies.Employees;
using GuldeLib.TypeObjects;
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
            var travelFactory = new TravelFactory(GameObject);
            travelFactory.Create(employee.Travel.Value);

            var personFactory = new PersonFactory(GameObject);
            personFactory.Create(employee.Person.Value);

            var employeeComponent = GameObject.AddComponent<EmployeeComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}