using System.Collections.Generic;
using System.Linq;
using GuldeLib.Companies;
using GuldeLib.Economy;
using GuldeLib.Extensions;
using GuldeLib.Maps;
using GuldeLib.Timing;
using GuldeLib.WorkerHomes;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using PlasticPipe.PlasticProtocol.Messages;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Cities
{
    /// <summary>
    /// Provides functionality for city management.
    /// </summary>
    [RequireComponent(typeof(MapComponent))]
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
        [BoxGroup("Debug")]
        public MarketComponent Market => this.GetCachedComponent<MarketComponent>();

        /// <summary>
        /// Gets the city's associated <see cref = "MapComponent">map</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public MapComponent Map => this.GetCachedComponent<MapComponent>();

        /// <summary>
        /// Gets the city's <see cref = "TimeComponent">time</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public TimeComponent Time => this.GetCachedComponent<TimeComponent>();

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

            Locator.City = this;
        }

        void Start()
        {
            Map.LocationRegistered += OnLocationRegistered;
        }

        /// <inheritdoc cref="MapComponent.LocationRegistered"/>
        void OnLocationRegistered(object sender, LocationEventArgs e)
        {
            this.Log($"City registered location {e.Location}");

            var companyComponent = e.Location.GetComponent<CompanyComponent>();
            var workerHomeComponent = e.Location.GetComponent<WorkerHomeComponent>();

            if (companyComponent) Companies.Add(companyComponent);
            if (workerHomeComponent) WorkerHomes.Add(workerHomeComponent);
        }
    }
}