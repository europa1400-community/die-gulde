using GuldeLib.Companies.Employees;
using GuldeLib.Extensions;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class EmployeeFactory : Factory<Employee, EmployeeComponent>
    {
        public EmployeeFactory(Employee employee, GameObject parentObject) : base(employee, null, parentObject)
        {
        }

        public override EmployeeComponent Create()
        {
            var travelFactory = new TravelFactory(TypeObject.Travel.Value, GameObject, ParentObject);
            var travelComponent = travelFactory.Create();

            var personFactory = new PersonFactory(TypeObject.Person.Value, GameObject);
            personFactory.Create();

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