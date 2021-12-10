using GuldeLib.Maps;
using GuldeLib.Maps.Buildings;
using GuldeLib.TypeObjects;
using GuldeLib.WorkerHomes;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class WorkerHomeFactory : Factory<WorkerHome, WorkerHomeComponent>
    {
        public WorkerHomeFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override WorkerHomeComponent Create(WorkerHome workerHome) => null;

        public WorkerHomeComponent Create(WorkerHome workerHome, MapComponent mapComponent)
        {
            var locationFactory = new LocationFactory(GameObject);
            locationFactory.Create(workerHome.Location.Value);

            var locationComponent = GameObject.GetComponent<LocationComponent>();
            var buildingComponent = GameObject.GetComponent<BuildingComponent>();

            var workerHomeEntryCell = mapComponent.MapLayout.PlaceBuilding(buildingComponent.Building);

            if (!workerHomeEntryCell.HasValue)
            {
                this.Log($"Could not create worker home: No build space available.", LogType.Warning);
                Object.Destroy(GameObject);
                return null;
            }

            mapComponent.Register(locationComponent);
            locationComponent.EntryCell = workerHomeEntryCell.Value;

            return Component;
        }
    }
}