using System;
using Gulde.Entities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Company.Employees
{
    [HideMonoScript]
    [RequireComponent(typeof(EntityComponent))]
    [RequireComponent(typeof(TravelComponent))]
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
        TravelComponent Travel { get; set; }

        void Awake()
        {
            Entity = GetComponent<EntityComponent>();
            Travel = GetComponent<TravelComponent>();

            if (Locator.Time)
            {
                Locator.Time.Morning -= OnMorning;
                Locator.Time.Evening -= OnEvening;

                Locator.Time.Morning += OnMorning;
                Locator.Time.Evening += OnEvening;
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
            if (Locator.City) Home = Locator.City.Map.GetNearestHome(company.Location);

            var startLocation = Home ? Home.Location : Company.Location;
            Travel.TravelTo(startLocation);
        }
    }
}