using System;
using GuldeLib.Companies.Carts;
using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.Pathfinding;
using GuldeLib.WorkerHomes;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Companies.Employees
{
    /// <summary>
    /// Provides information and behavior for employees.
    /// </summary>
    public class EmployeeComponent : SerializedMonoBehaviour
    {

        /// <summary>
        /// Gets or sets the <see cref = "EmployeeComponent">Employee's</see> <see cref = "WorkerHomeComponent">WorkerHome</see>.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        WorkerHomeComponent Home { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "EmployeeComponent">Employee's</see> <see cref = "CompanyComponent">Company</see>.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        CompanyComponent Company { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "EmployeeComponent">Employee's</see> <see cref = "EntityComponent">Entity</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public EntityComponent Entity => GetComponent<EntityComponent>();

        /// <summary>
        /// Gets or sets the <see cref = "EmployeeComponent">Employee's</see> <see cref = "TravelComponent">TravelComponent</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public TravelComponent Travel => GetComponent<TravelComponent>();

        /// <summary>
        /// Gets or sets the <see cref = "EmployeeComponent">Employee's</see> <see cref = "PathfinderComponent">PathfindingComponent</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        PathfinderComponent Pathfinder => GetComponent<PathfinderComponent>();

        /// <summary>
        /// Gets whether the <see cref = "EmployeeComponent">Employee</see> is located at its company.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        public bool IsAtCompany => Company && Entity.Location == Company.Location;

        /// <summary>
        /// Invoked after the <see cref = "EmployeeComponent">Employee's</see> location changes to its company.
        /// </summary>
        public event EventHandler CompanyReached;

        /// <summary>
        /// Invoked after the <see cref = "EmployeeComponent">Employee's</see> location changes to its worker home.
        /// </summary>
        public event EventHandler HomeReached;

        public event EventHandler<InitializedEventArgs> Initialized;

        void Awake()
        {
            HomeReached += OnHomeReached;
        }

        void Start()
        {
            Initialized?.Invoke(this, new InitializedEventArgs());
        }

        /// <summary>
        /// Callback for the <see cref = "TravelComponent.DestinationReached">DestinationReached</see> event.<br/>
        /// Invokes the <see cref = "CompanyReached">CompanyReached</see> or <see cref = "HomeReached">HomeReached</see> event.
        /// </summary>
        public void OnDestinationReached(object sender, MapComponent.LocationEventArgs e)
        {
            if (e.Location == Company.Location)
            {
                this.Log($"Employee reached company {Company}");
                CompanyReached?.Invoke(this, EventArgs.Empty);
            }

            if (e.Location == Home.Location)
            {
                this.Log($"Employee reached home {Home}");
                HomeReached?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Callback for the <see cref = "HomeReached">HomeReached</see> event.<br/>
        /// Sends the <see cref = "EmployeeComponent">Employee</see> back to its company during <see cref = "GuldeLib.Timing.TimeComponent.IsWorkingHour">working hours</see>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHomeReached(object sender, EventArgs e)
        {
            if (!Locator.Time || !Locator.Time.IsWorkingHour) return;

            this.Log("Employee reached home but it was work time", LogType.Warning);

            Travel.TravelTo(Company.Location);
        }

        /// <summary>
        /// Callback for the <see cref = "Timing.TimeComponent.Morning">Morning</see> event of the <see cref = "Timing.TimeComponent">TimeComponent</see>.<br/>
        /// Sends the <see cref = "EmployeeComponent">Employee</see> to its company.
        /// </summary>
        public void OnMorning(object sender, EventArgs e)
        {
            this.Log("Employee is travelling to work");
            Travel.TravelTo(Company.Location);
        }

        /// <summary>
        /// Callback for the <see cref = "Timing.TimeComponent.Evening">Evening</see> event of the <see cref = "Timing.TimeComponent">TimeComponent</see>.<br/>
        /// Sends the <see cref = "EmployeeComponent">Employee</see> to its worker home.
        /// </summary>
        public void OnEvening(object sender, EventArgs e)
        {
            this.Log("Employee is travelling home");
            Travel.TravelTo(Home.Location);
        }

        /// <summary>
        /// Sets the <see cref = "EmployeeComponent">Employee's</see> company,
        /// sets its worker home to the one closest to the company
        /// and spawns it at that worker home.
        /// </summary>
        /// <param name="company">The <see cref = "CompanyComponent">Company</see> hiring the <see cref = "EmployeeComponent">Employee</see>.</param>
        public void SetCompany(CompanyComponent company)
        {
            Company = company;
            this.Log($"Employee was hired for company {Company} at {Company.Location.EntryCell}");

            if (Locator.City)
            {
                var nearestHome = Locator.City.GetNearestHome(company.Location);

                Home = nearestHome;

                if (Home) this.Log($"Employee chose home {Home.name} at {Home.Location.EntryCell}");
                else this.Log($"Employee could not find a valid home.", LogType.Warning);
            }

            var startLocation = Home ? Home.Location : Company.Location;

            this.Log($"Employee is spawning at {startLocation}");
            Travel.TravelTo(startLocation);
        }

        public class InitializedEventArgs : EventArgs
        {
        }

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
}