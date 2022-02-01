using GuldeLib.Companies.Employees;
using GuldeLib.Generators;
using GuldeLib.Pathfinding;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class EmployeeBuilder : Builder<Employee>
    {
        public EmployeeBuilder WithPerson(GeneratablePerson person)
        {
            Object.Person = person;
            return this;
        }

        public EmployeeBuilder WithTravel(GeneratableTravel travel)
        {
            Object.Travel = travel;
            return this;
        }
    }
}