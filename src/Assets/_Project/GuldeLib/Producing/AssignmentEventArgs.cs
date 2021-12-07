using System;
using GuldeLib.Companies.Employees;
using GuldeLib.TypeObjects;

namespace GuldeLib.Producing
{
    public class AssignmentEventArgs : EventArgs
    {
        public AssignmentEventArgs(EmployeeComponent employee, Recipe recipe)
        {
            Employee = employee;
            Recipe = recipe;
        }

        public EmployeeComponent Employee { get; }

        public Recipe Recipe { get; }
    }
}