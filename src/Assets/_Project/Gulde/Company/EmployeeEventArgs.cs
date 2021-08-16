using System;
using Gulde.Production;

namespace Gulde.Company
{
    public class EmployeeEventArgs : EventArgs
    {
        public EmployeeComponent Employee;

        public EmployeeEventArgs(EmployeeComponent employee)
        {
            Employee = employee;
        }
    }
}