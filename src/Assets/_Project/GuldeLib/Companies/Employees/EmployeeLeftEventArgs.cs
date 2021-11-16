using System;

namespace GuldeLib.Companies.Employees
{
    /// <summary>
    /// Contains arguments for the <see cref = "CompanyComponent.EmployeeLeft"/> event.
    /// </summary>
    public class EmployeeLeftEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref = "EmployeeComponent">Employee</see> who has left the company.
        /// </summary>
        public EmployeeComponent Employee { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref = "EmployeeLeftEventArgs">EmployeeLeftEventArgs</see> class.
        /// </summary>
        /// <param name="employee">The <see cref = "EmployeeComponent">Employee</see> who has left the company.</param>
        public EmployeeLeftEventArgs(EmployeeComponent employee)
        {
            Employee = employee;
        }
    }
}