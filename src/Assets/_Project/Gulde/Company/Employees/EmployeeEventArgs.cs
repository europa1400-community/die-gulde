using System;
using Gulde.Company.Employees;
using Gulde.Production;

namespace Gulde.Company
{
    public class EmployeeEventArgs : EventArgs
    {
        public EmployeeComponent Employee { get; }

        public EmployeeEventArgs(EmployeeComponent employee)
        {
            Employee = employee;
        }
    }
}