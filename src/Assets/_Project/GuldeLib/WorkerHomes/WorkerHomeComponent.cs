using System;
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
    public class WorkerHomeComponent : SerializedMonoBehaviour
    {
        /// <summary>
        /// Gets the <see cref = "LocationComponent">LocationComponent</see> of the worker home.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public LocationComponent Location => GetComponent<LocationComponent>();

        public event EventHandler<InitializedEventArgs> Initialized;

        void Start()
        {
            Initialized?.Invoke(this, new InitializedEventArgs());
        }

        public class InitializedEventArgs : EventArgs
        {

        }
    }
}