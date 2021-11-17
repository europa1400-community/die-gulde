using GuldeLib.WorkerHomes;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class WorkerHomeFactory : Factory<WorkerHome>
    {
        public WorkerHomeFactory(GameObject gameObject, GameObject parentObject) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(WorkerHome workerHome)
        {
            var locationFactory = new LocationFactory(GameObject);
            locationFactory.Create(workerHome.Location);

            var workerHomeComponent = GameObject.AddComponent<WorkerHomeComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}