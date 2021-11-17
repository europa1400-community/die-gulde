using GuldeLib.Companies.Employees;
using GuldeLib.Pathfinding;
using GuldeLib.Persons;

namespace GuldeLib.Builders
{
    public class EmployeeBuilder : Builder<Employee>
    {
        public EmployeeBuilder WithPerson(Person person)
        {
            Object.Person = person;
            return this;
        }

        public EmployeeBuilder WithPathfinder(Pathfinder pathfinder)
        {
            Object.Pathfinder = pathfinder;
            return this;
        }
    }
}