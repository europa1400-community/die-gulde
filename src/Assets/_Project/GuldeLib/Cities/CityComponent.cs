using System.Collections.Generic;
using System.Linq;
using GuldeLib.Company;
using GuldeLib.Company.Employees;
using GuldeLib.Economy;
using GuldeLib.Entities.Pathfinding;
using GuldeLib.Maps;
using GuldeLib.Timing;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Cities
{
    [RequireComponent(typeof(NavComponent))]
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
        }
    }
}