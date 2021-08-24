using System.Collections;
using Gulde.Company.Employees;
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

        public WorkerHomeBuilder() : base()
        {
        }

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

        public override IEnumerator Build()
        {
            yield return base.Build();

            var parent = Parent ? Parent.transform : Map ? Map.transform : null;
            WorkerHomeObject = Object.Instantiate(WorkerHomePrefab, parent);

            var workerHome = WorkerHomeObject.GetComponent<WorkerHomeComponent>();
            workerHome.Location.SetContainingMap(Map);
        }
    }
}