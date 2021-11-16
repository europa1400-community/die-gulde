using System;
using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.Pathfinding;
using GuldeLib.WorkerHomes;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Companies.Employees
{
    /// <summary>
    /// Provides information and behavior for employees.
    /// </summary>
    [RequireComponent(typeof(EntityComponent))]
    [RequireComponent(typeof(TravelComponent))]
    [RequireComponent(typeof(PathfindingComponent))]
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
        public EntityComponent Entity { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "EmployeeComponent">Employee's</see> <see cref = "TravelComponent">TravelComponent</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public TravelComponent Travel { get; set; }

        /// <summary>
        /// Gets or sets the <see cref = "EmployeeComponent">Employee's</see> <see cref = "PathfindingComponent">PathfindingComponent</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        PathfindingComponent Pathfinding { get; set; }

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

        /// <summary>
        /// Returns a new <see cref = "WaitForCompanyReached">WaitForCompanyReached</see> <see cref = "CustomYieldInstruction">CustomYieldInstruction</see>
        /// that waits for the <see cref = "CompanyReached">CompanyReached</see> event to be invoked.
        /// </summary>
        public WaitForCompanyReached WaitForCompanyReached => new WaitForCompanyReached(this);

        /// <summary>
        /// Returns a new <see cref = "WaitForCompanyReached">WaitForCompanyReached</see> <see cref = "CustomYieldInstruction">CustomYieldInstruction</see>
        /// that waits for the <see cref = "HomeReached">HomeReached</see> event to be invoked.
        /// </summary>
        public WaitForHomeReached WaitForHomeReached => new WaitForHomeReached(this);

        void Awake()
        {
            this.Log("Employee created");

            Entity = GetComponent<EntityComponent>();
            Travel = GetComponent<TravelComponent>();
            Pathfinding = GetComponent<PathfindingComponent>();

            HomeReached += OnHomeReached;
            Travel.DestinationReached += OnDestinationReached;

            if (Locator.Time)
            {
                Locator.Time.Morning -= OnMorning;
                Locator.Time.Evening -= OnEvening;

                Locator.Time.Morning += OnMorning;
                Locator.Time.Evening += OnEvening;
            }
        }

        /// <summary>
        /// Callback for the <see cref = "TravelComponent.DestinationReached">DestinationReached</see> event.<br/>
        /// Invokes the <see cref = "CompanyReached">CompanyReached</see> or <see cref = "HomeReached">HomeReached</see> event.
        /// </summary>
        void OnDestinationReached(object sender, LocationEventArgs e)
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
        void OnMorning(object sender, EventArgs e)
        {
            this.Log("Employee is travelling to work");
            Travel.TravelTo(Company.Location);
        }


        /// <summary>
        /// Callback for the <see cref = "Timing.TimeComponent.Evening">Evening</see> event of the <see cref = "Timing.TimeComponent">TimeComponent</see>.<br/>
        /// Sends the <see cref = "EmployeeComponent">Employee</see> to its worker home.
        /// </summary>
        void OnEvening(object sender, EventArgs e)
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
                this.Log($"Employee chose home {Home.name} at {Home.Location.EntryCell}");
            }

            var startLocation = Home ? Home.Location : Company.Location;

            this.Log($"Employee is spawning at {startLocation}");
            Travel.TravelTo(startLocation);
        }
    }
}