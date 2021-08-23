using System;
using Gulde.Entities;
using Gulde.Maps;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Company
{
    [HideMonoScript]
    [RequireComponent(typeof(EntityComponent))]
    [RequireComponent(typeof(TravelComponent))]
    public class EmployeeComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [Required]
        LocationComponent HomeLocation { get; set; }

        [OdinSerialize]
        [Required]
        LocationComponent WorkLocation { get; set; }

        [OdinSerialize]
        [ReadOnly]
        public EntityComponent Entity { get; set; }

        [OdinSerialize]
        [ReadOnly]
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
            Travel.TravelTo(WorkLocation);
        }

        void OnEvening(object sender, EventArgs e)
        {
            Travel.TravelTo(HomeLocation);
        }
    }
}