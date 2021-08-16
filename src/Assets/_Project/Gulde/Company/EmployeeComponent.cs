using System;
using Gulde.Entities;
using Gulde.Maps;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        EntityComponent Entity { get; set; }

        [OdinSerialize]
        [ReadOnly]
        TravelComponent Travel { get; set; }

        void Awake()
        {
            Entity = GetComponent<EntityComponent>();
            Travel = GetComponent<TravelComponent>();

            Locator.TimeComponent.Morning -= OnMorning;
            Locator.TimeComponent.Evening -= OnEvening;

            Locator.TimeComponent.Morning += OnMorning;
            Locator.TimeComponent.Evening += OnEvening;
        }

        void OnMorning(object sender, EventArgs e)
        {
            if (!WorkLocation)
            {
                Debug.Log($"{name} has no WorkLocation! wtf");
            }
            Travel.TravelTo(WorkLocation);
        }

        void OnEvening(object sender, EventArgs e)
        {
            Travel.TravelTo(HomeLocation);
        }

        #region OdinInspector

        #if UNITY_EDITOR

        void OnValidate()
        {
            if (!gameObject.scene.isLoaded) return;

            Entity = GetComponent<EntityComponent>();
            Travel = GetComponent<TravelComponent>();

            Locator.TimeComponent.Morning -= OnMorning;
            Locator.TimeComponent.Evening -= OnEvening;

            Locator.TimeComponent.Morning += OnMorning;
            Locator.TimeComponent.Evening += OnEvening;
        }

        #endif

        #endregion
    }
}