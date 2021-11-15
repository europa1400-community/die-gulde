using System;

namespace GuldeLib.Company.Employees
{
    /// <summary>
    /// Contains arguments for the <see cref = "CompanyComponent.EmployeeArrived"/> event.
    /// </summary>
    public class EmployeeArrivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref = "EmployeeComponent">Employee</see> who has arrived at the company.
        /// </summary>
        public EmployeeComponent Employee { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref = "EmployeeArrivedEventArgs">EmployeeArrivedEventArgs</see> class.
        /// </summary>
        /// <param name="employee">The <see cref = "EmployeeComponent">Employee</see> who has arrived at the company.</param>
        public EmployeeArrivedEventArgs(EmployeeComponent employee)
        {
            Employee = employee;
        }
    }
}