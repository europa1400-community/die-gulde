using System;
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
using Sirenix.Utilities;
using UnityEngine;

namespace GuldeLib.Cities
{
    /// <summary>
    /// Provides functionality for city management.
    /// </summary>
    public class CityComponent : SerializedMonoBehaviour
    {
        /// <summary>
        /// Gets the set of worker homes contained in the city.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<WorkerHomeComponent> WorkerHomes =>
            Map
                ? Map.Locations
                    .Where(e => e.GetComponent<WorkerHomeComponent>())
                    .Select(e => e.GetComponent<WorkerHomeComponent>())
                    .ToHashSet()
                : new HashSet<WorkerHomeComponent>();

        /// <summary>
        /// Gets the set of companies contained in the city.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<CompanyComponent> Companies =>
            Map
                ? Map.Locations
                    .Where(e => e.GetComponent<CompanyComponent>())
                    .Select(e => e.GetComponent<CompanyComponent>())
                    .ToHashSet()
                : new HashSet<CompanyComponent>();

        /// <summary>
        /// Gets the city's <see cref = "MarketComponent">market</see>.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Debug")]
        public MarketComponent Market => GetComponent<MarketComponent>();

        /// <summary>
        /// Gets the city's associated <see cref = "MapComponent">map</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public MapComponent Map => GetComponent<MapComponent>();

        /// <summary>
        /// Gets the city's <see cref = "TimeComponent">time</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public TimeComponent Time => GetComponent<TimeComponent>();

        public event EventHandler<InitializedEventArgs> Initialized;

        void Start()
        {
            Initialized?.Invoke(this, new InitializedEventArgs());
        }

        /// <summary>
        /// Gets the <see cref = "WorkerHomeComponent">worker home</see> with an entry cell nearest to the specified <see cref = "LocationComponent">location's</see> entry cell.
        /// </summary>
        public WorkerHomeComponent GetNearestHome(LocationComponent from) =>
            WorkerHomes
                .OrderBy(workerHome => workerHome.Location.EntryCell.DistanceTo(from.EntryCell))
                .FirstOrDefault();

        public class InitializedEventArgs : EventArgs
        {

        }
    }
}