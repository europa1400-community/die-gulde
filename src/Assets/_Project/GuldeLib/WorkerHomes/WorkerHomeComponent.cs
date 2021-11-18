using GuldeLib.Maps;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.WorkerHomes
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
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public LocationComponent Location => this.GetCachedComponent<LocationComponent>();

        void Awake()
        {
            this.Log("Worker home created");
        }
    }
}