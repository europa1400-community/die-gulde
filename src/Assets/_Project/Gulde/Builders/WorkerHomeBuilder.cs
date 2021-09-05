using System.Collections;
using Gulde.Company.Employees;
using UnityExtensions.Runtime;
using Gulde.Logging;
using Gulde.Maps;
using UnityEngine;

namespace Gulde.Builders
{
    public class WorkerHomeBuilder : Builder
    {
        public GameObject WorkerHomeObject { get; private set; }

        [LoadAsset("prefab_worker_home")]
        GameObject WorkerHomePrefab { get; set; }

        MapComponent Map { get; set; }
        GameObject Parent { get; set; }
        Vector3Int EntryCell { get; set; }

        public WorkerHomeBuilder() : base() { }

        public WorkerHomeBuilder WithMap(MapComponent map)
        {
            Map = map;
            return this;
        }

        public WorkerHomeBuilder WithParent(GameObject parent)
        {
            Parent = parent;
            return this;
        }

        public WorkerHomeBuilder WithEntryCell(int x, int y)
        {
            EntryCell = new Vector3Int(x, y, 0);
            return this;
        }

        public WorkerHomeBuilder WithEntryCell(Vector3Int cell)
        {
            EntryCell = cell;
            return this;
        }

        public override IEnumerator Build()
        {
            // if (!Map)
            // {
            //     this.Log("Cannot create worker home without map.", LogType.Error);
            //     yield break;
            // }

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