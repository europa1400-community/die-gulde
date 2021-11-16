using System.Collections;
using GuldeLib.Maps;
using GuldeLib.WorkerHomes;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Builders
{
    /// <summary>
    /// Provides functionality to build a worker home.
    /// </summary>
    public class WorkerHomeBuilder : Builder
    {
        /// <summary>
        /// Gets the build worker home's GameObject.
        /// </summary>
        public GameObject WorkerHomeObject { get; private set; }

        /// <summary>
        /// Gets or sets the prefab used to build the worker home.
        /// </summary>
        [LoadAsset("prefab_worker_home")]
        GameObject WorkerHomePrefab { get; set; }

        /// <inheritdoc cref="LocationComponent.ContainingMap"/>
        MapComponent Map { get; set; }

        /// <inheritdoc cref="Transform.parent"/>
        GameObject Parent { get; set; }

        /// <inheritdoc cref="LocationComponent.EntryCell"/>
        Vector3Int EntryCell { get; set; }

        /// <summary>
        /// Sets the worker home's containing map to the given value.
        /// </summary>
        public WorkerHomeBuilder WithMap(MapComponent map)
        {
            Map = map;
            return this;
        }

        /// <summary>
        /// Sets the worker home's parent transform to the given value.
        /// </summary>
        public WorkerHomeBuilder WithParent(GameObject parent)
        {
            Parent = parent;
            return this;
        }

        /// <summary>
        /// Sets the worker home's entry cell position to the given value.
        /// </summary>
        /// <param name="x">The x coordinate in cells.</param>
        /// <param name="y">The y coordinate in cells.</param>
        public WorkerHomeBuilder WithEntryCell(int x, int y)
        {
            EntryCell = new Vector3Int(x, y, 0);
            return this;
        }

        /// <summary>
        /// Sets the worker home's entry cell position to the given value.
        /// </summary>
        public WorkerHomeBuilder WithEntryCell(Vector3Int cell)
        {
            EntryCell = cell;
            return this;
        }

        /// <inheritdoc cref="Builder.Build"/>
        public override IEnumerator Build()
        {
            if (!EntryCell.IsInBounds(Map.Size))
            {
                this.Log($"Cannot create worker home out of bounds at {EntryCell}", LogType.Error);
                yield break;
            }

            yield return base.Build();

            var parent = Parent ? Parent.transform : Map.transform;
            WorkerHomeObject = Object.Instantiate(WorkerHomePrefab, parent);

            var workerHome = WorkerHomeObject.GetComponent<WorkerHomeComponent>();
            var location = WorkerHomeObject.GetComponent<LocationComponent>();

            workerHome.Location.EntryCell = EntryCell;
            if (Map) Map.Register(location);
        }
    }
}