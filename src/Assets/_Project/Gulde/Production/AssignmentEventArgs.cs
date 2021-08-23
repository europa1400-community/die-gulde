using System;
using Gulde.Company;
using Gulde.Company.Employees;

namespace Gulde.Production
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