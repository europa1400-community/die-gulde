using GuldeLib.Maps;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Company.Employees
{
    /// <summary>
    /// Provides information and behavior for worker homes.
    /// </summary>
    [RequireComponent(typeof(LocationComponent))]
    public class WorkerHomeComponent : SerializedMonoBehaviour
    {
        /// <summary>
        /// Gets the <see cref = "LocationComponent">LocationComponent</see> of the worker home.
        /// </summary>
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