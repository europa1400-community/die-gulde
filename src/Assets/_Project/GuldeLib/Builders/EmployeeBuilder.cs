using GuldeLib.Companies.Employees;
using GuldeLib.Generators;
using GuldeLib.Pathfinding;
using GuldeLib.Persons;

namespace GuldeLib.Builders
{
    public class EmployeeBuilder : Builder<Employee>
    {
        public EmployeeBuilder WithPerson(GeneratablePerson person)
        {
            Object.Person = person;
            return this;
        }

        public EmployeeBuilder WithPathfinder(GeneratablePathfinder pathfinder)
        {
            Object.Pathfinder = pathfinder;
            return this;
        }
    }
}