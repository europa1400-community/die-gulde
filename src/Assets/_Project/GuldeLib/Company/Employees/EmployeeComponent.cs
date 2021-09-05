using System;
using GuldeLib.Entities;
using GuldeLib.Entities.Pathfinding;
using GuldeLib.Maps;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Company.Employees
{
    [RequireComponent(typeof(EntityComponent))]
    [RequireComponent(typeof(TravelComponent))]
    [RequireComponent(typeof(PathfindingComponent))]
    public class EmployeeComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Info")]
        WorkerHomeComponent Home { get; set; }

        [OdinSerialize]
        [BoxGroup("Info")]
        CompanyComponent Company { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public EntityComponent Entity { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public TravelComponent Travel { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        PathfindingComponent Pathfinding { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public bool IsAtCompany => Company && Entity.Location == Company.Location;

        public event EventHandler CompanyReached;
        public event EventHandler HomeReached;

        public WaitForCompanyReached WaitForCompanyReached => new WaitForCompanyReached(this);
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

        void OnHomeReached(object sender, EventArgs e)
        {
            if (!Locator.Time || !Locator.Time.IsWorkingHour) return;

            this.Log("Employee reached home but it was work time", LogType.Warning);

            Travel.TravelTo(Company.Location);
        }

        void OnMorning(object sender, EventArgs e)
        {
            this.Log("Employee is travelling to work");
            Travel.TravelTo(Company.Location);
        }

        void OnEvening(object sender, EventArgs e)
        {
            this.Log("Employee is travelling home");
            Travel.TravelTo(Home.Location);
        }

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

    public class WaitForCompanyReached : CustomYieldInstruction
    {
        EmployeeComponent Employee { get; }
        bool HasReachedCompany { get; set; }

        public override bool keepWaiting => !HasReachedCompany && !Employee.IsAtCompany;

        public WaitForCompanyReached(EmployeeComponent employee)
        {
            Employee = employee;
            Employee.CompanyReached += OnCompanyReached;
        }

        void OnCompanyReached(object sender, EventArgs e)
        {
            HasReachedCompany = true;
        }
    }

    public class WaitForHomeReached : CustomYieldInstruction
    {
        EmployeeComponent Employee { get; }
        bool HasReachedHome { get; set; }

        public override bool keepWaiting => !HasReachedHome;

        public WaitForHomeReached(EmployeeComponent employee)
        {
            Employee = employee;
            Employee.HomeReached += OnHomeReached;
        }

        void OnHomeReached(object sender, EventArgs e)
        {
            HasReachedHome = true;
        }
    }
}