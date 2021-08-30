using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Company;
using Gulde.Company.Employees;
using Gulde.Economy;
using Gulde.Entities;
using Gulde.Extensions;
using Gulde.Logging;
using Gulde.Maps;
using Gulde.Timing;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Cities
{
    [RequireComponent(typeof(MapComponent))]
    [RequireComponent(typeof(TimeComponent))]
    public class CityComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<WorkerHomeComponent> WorkerHomes { get; } = new HashSet<WorkerHomeComponent>();

        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<CompanyComponent> Companies { get; } = new HashSet<CompanyComponent>();

        [ShowInInspector]
        [BoxGroup("Info")]
        public MarketComponent Market { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public MapComponent Map { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public TimeComponent Time { get; private set; }

        public event EventHandler<LocationEventArgs> LocationRegistered;

        public WorkerHomeComponent GetNearestHome(LocationComponent from) =>
            WorkerHomes
                .OrderBy(workerHome => workerHome.Location.EntryCell.DistanceTo(from.EntryCell))
                .FirstOrDefault();

        void Awake()
        {
            this.Log("City created");

            Map = GetComponent<MapComponent>();
            Time = GetComponent<TimeComponent>();

            Locator.City = this;

            Map.LocationRegistered += OnLocationRegistered;
        }

        void OnLocationRegistered(object sender, LocationEventArgs e)
        {
            this.Log($"City registered location {e.Location}");

            var companyComponent = e.Location.GetComponent<CompanyComponent>();
            var workerHomeComponent = e.Location.GetComponent<WorkerHomeComponent>();
            var marketComponent = e.Location.GetComponent<MarketComponent>();

            if (companyComponent) Companies.Add(companyComponent);
            if (workerHomeComponent) WorkerHomes.Add(workerHomeComponent);
            if (marketComponent) Market = marketComponent;

            LocationRegistered?.Invoke(this, new LocationEventArgs(e.Location));
        }
    }
}