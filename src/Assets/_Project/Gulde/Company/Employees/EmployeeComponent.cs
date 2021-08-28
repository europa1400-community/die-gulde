using System;
using Gulde.Entities;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Company.Employees
{
    [HideMonoScript]
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

        public bool IsAtCompany => Entity.Location == Company.Location;
        public bool IsWorking => IsAtCompany || Company.Assignment.IsAssignedExternally(this);

        public event EventHandler CompanyReached;

        public WaitForCompanyReached WaitForCompanyReached => new WaitForCompanyReached(this);

        void Awake()
        {
            Entity = GetComponent<EntityComponent>();
            Travel = GetComponent<TravelComponent>();
            Pathfinding = GetComponent<PathfindingComponent>();

            Travel.LocationReached += OnLocationReached;

            if (Locator.Time)
            {
                Locator.Time.Morning -= OnMorning;
                Locator.Time.Evening -= OnEvening;

                Locator.Time.Morning += OnMorning;
                Locator.Time.Evening += OnEvening;
            }
        }

        void OnLocationReached(object sender, LocationEventArgs e)
        {
            if (e.Location == Company.Location)
            {
                CompanyReached?.Invoke(this, EventArgs.Empty);
            }

            else if (e.Location == Home.Location && Locator.Time && Locator.Time.IsWorkingHour)
            {
                Travel.TravelTo(Company.Location);
            }
        }

        void OnMorning(object sender, EventArgs e)
        {
            Travel.TravelTo(Company.Location);
        }

        void OnEvening(object sender, EventArgs e)
        {
            Travel.TravelTo(Home.Location);
        }

        public void SetCompany(CompanyComponent company)
        {
            Company = company;

            Debug.Log($"Set employee company to {Company.name}");
            if (Locator.City)
            {
                var nearestHome = Locator.City.GetNearestHome(company.Location);
                Home = nearestHome;

                Debug.Log($"Set employee home to {Home.name}");
            }

            var startLocation = Home ? Home.Location : Company.Location;

            Debug.Log($"Travelling to {startLocation.name}");
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
}