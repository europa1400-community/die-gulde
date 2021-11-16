using System;
using UnityEngine;

namespace GuldeLib.Company.Employees
{
    /// <summary>
    /// <see cref = "CustomYieldInstruction">CustomYieldInstruction</see> that waits for
    /// the <see cref = "EmployeeComponent.CompanyReached">CompanyReached</see> event
    /// of a given <see cref = "EmployeeComponent">EmployeeComponent</see> to be invoked.
    /// </summary>
    public class WaitForCompanyReached : CustomYieldInstruction
    {
        /// <summary>
        /// Gets the <see cref = "EmployeeComponent">Employee</see> whose <see cref = "EmployeeComponent.CompanyReached">CompanyReached</see> is being waited for.
        /// </summary>
        EmployeeComponent Employee { get; }

        /// <summary>
        /// Gets or sets whether the <see cref = "EmployeeComponent.CompanyReached">CompanyReached</see> event has been called.
        /// </summary>
        bool HasReachedCompany { get; set; }

        /// <inheritdoc cref="keepWaiting"/>
        public override bool keepWaiting => !HasReachedCompany && !Employee.IsAtCompany;

        /// <summary>
        /// Initializes a new instance of the <see cref = "WaitForCompanyReached">WaitForCompanyReached</see> <see cref = "CustomYieldInstruction">CustomYieldInstruction</see>.
        /// </summary>
        /// <param name="employee">The <see cref = "EmployeeComponent">Employee</see> whose <see cref = "EmployeeComponent.CompanyReached">CompanyReached</see> event is being waited for.</param>
        public WaitForCompanyReached(EmployeeComponent employee)
        {
            Employee = employee;
            Employee.CompanyReached += OnCompanyReached;
        }

        /// <summary>
        /// Callback for the <see cref = "EmployeeComponent.CompanyReached">CompanyReached</see> event
        /// of the given <see cref = "EmployeeComponent">EmployeeComponent</see>.<br/>
        /// Sets <see cref = "HasReachedCompany">HasReachedCompany</see> to true.
        /// </summary>
        void OnCompanyReached(object sender, EventArgs e)
        {
            HasReachedCompany = true;
        }
    }
}