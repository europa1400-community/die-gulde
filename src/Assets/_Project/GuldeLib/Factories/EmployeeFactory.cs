using GuldeLib.Companies.Employees;
using GuldeLib.Extensions;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class EmployeeFactory : Factory<Employee, EmployeeComponent>
    {
        public EmployeeFactory(GameObject parentObject) : base(null, parentObject)
        {
        }

        public override EmployeeComponent Create(Employee employee)
        {
            var travelFactory = new TravelFactory(GameObject, ParentObject);
            var travelComponent = travelFactory.Create(employee.Travel.Value);

            var personFactory = new PersonFactory(GameObject);
            personFactory.Create(employee.Person.Value);

            travelComponent.DestinationReached += Component.OnDestinationReached;

            if (Locator.Time)
            {
                Locator.Time.Morning += Component.OnMorning;
                Locator.Time.Evening += Component.OnEvening;
            }

            return Component;
        }
    }
}