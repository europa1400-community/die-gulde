using GuldeLib.Maps;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Company.Employees
{
    [RequireComponent(typeof(LocationComponent))]
    public class WorkerHomeComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public LocationComponent Location { get; private set; }

        void Awake()
        {
            this.Log("Worker home created");
            Location = GetComponent<LocationComponent>();
        }
    }
}