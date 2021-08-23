using System;
using Gulde.Company;

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