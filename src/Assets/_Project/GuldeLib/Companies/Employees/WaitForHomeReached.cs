using System;
using UnityEngine;

namespace GuldeLib.Companies.Employees
{
    /// <summary>
    /// <see cref = "CustomYieldInstruction">CustomYieldInstruction</see> that waits for
    /// the <see cref = "EmployeeComponent.HomeReached">HomeReached</see> event
    /// of a given <see cref = "EmployeeComponent">EmployeeComponent</see> to be invoked.
    /// </summary>
    public class WaitForHomeReached : CustomYieldInstruction
    {
        /// <summary>
        /// Gets the <see cref = "EmployeeComponent">Employee</see> whose <see cref = "EmployeeComponent.HomeReached">HomeReached</see> is being waited for.
        /// </summary>
        EmployeeComponent Employee { get; }

        /// <summary>
        /// Gets or sets whether the <see cref = "EmployeeComponent.HomeReached">HomeReached</see> event has been called.
        /// </summary>
        bool HasReachedHome { get; set; }
        
        /// <inheritdoc cref="keepWaiting"/>
        public override bool keepWaiting => !HasReachedHome;

        /// <summary>
        /// Initializes a new instance of the <see cref = "WaitForHomeReached">WaitForHomeReached</see> <see cref = "CustomYieldInstruction">CustomYieldInstruction</see>.
        /// </summary>
        /// <param name="employee">The <see cref = "EmployeeComponent">Employee</see> whose <see cref = "EmployeeComponent.HomeReached">HomeReached</see> event is being waited for.</param>
        public WaitForHomeReached(EmployeeComponent employee)
        {
            Employee = employee;
            Employee.HomeReached += OnHomeReached;
        }

        /// <summary>
        /// Callback for the <see cref = "EmployeeComponent.HomeReached">HomeReached</see> event
        /// of the given <see cref = "EmployeeComponent">EmployeeComponent</see>.<br/>
        /// Sets <see cref = "HasReachedHome">HasReachedHome</see> to true.
        /// </summary>
        void OnHomeReached(object sender, EventArgs e)
        {
            HasReachedHome = true;
        }
    }
}