using System.Collections.Generic;
using System.Linq;
using GuldeLib.Company;
using GuldeLib.Company.Employees;
using GuldeLib.Economy;
using GuldeLib.Entities.Pathfinding;
using GuldeLib.Maps;
using GuldeLib.Timing;
using GuldeLib.WorkerHomes;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Cities
{
    /// <summary>
    /// Provides functionality for city management.
    /// </summary>
    [RequireComponent(typeof(MapComponent))]
    [RequireComponent(typeof(NavComponent))]
    [RequireComponent(typeof(TimeComponent))]
    public class CityComponent : SerializedMonoBehaviour
    {
        /// <summary>
        /// Gets the set of worker homes contained in the city.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<WorkerHomeComponent> WorkerHomes { get; } = new HashSet<WorkerHomeComponent>();

        /// <summary>
        /// Gets the set of companies contained in the city.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<CompanyComponent> Companies { get; } = new HashSet<CompanyComponent>();

        /// <summary>
        /// Gets the city's <see cref = "MarketComponent">market</see>.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        public MarketComponent Market { get; private set; }

        /// <summary>
        /// Gets the city's associated <see cref = "MapComponent">map</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public MapComponent Map { get; private set; }

        /// <summary>
        /// Gets the city's <see cref = "TimeComponent">time</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public TimeComponent Time { get; private set; }

        /// <summary>
        /// Gets the <see cref = "WorkerHomeComponent">worker home</see> with an entry cell nearest to the specified <see cref = "LocationComponent">location's</see> entry cell.
        /// </summary>
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

        /// <inheritdoc cref="MapComponent.LocationRegistered"/>
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