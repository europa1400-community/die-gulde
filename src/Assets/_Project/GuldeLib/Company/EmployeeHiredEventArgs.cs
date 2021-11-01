using System;
using GuldeLib.Company.Employees;
using GuldeLib.Entities;

namespace GuldeLib.Company
{
    /// <summary>
    /// Contains arguments for the <see cref = "CompanyComponent.EmployeeHired">EmployeeHired</see> event
    /// of the <see cref = "CompanyComponent">CompanyComponent</see>.
    /// </summary>
    public class EmployeeHiredEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the hired <see cref = "EmployeeComponent">Employee</see>.
        /// </summary>
        public EmployeeComponent Employee { get; }
        /// <summary>
        /// Gets the cost of hiring the employee.
        /// </summary>
        public int Cost { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref = "EmployeeHiredEventArgs">EmployeeHiredEventArgs</see> class.
        /// </summary>
        /// <param name="employee">The hired <see cref = "EmployeeComponent">Employee</see>.</param>
        /// <param name="cost">The cost of hiring the employee.</param>
        public EmployeeHiredEventArgs(EmployeeComponent employee, int cost)
        {
            Employee = employee;
            Cost = cost;
        }
    }
}