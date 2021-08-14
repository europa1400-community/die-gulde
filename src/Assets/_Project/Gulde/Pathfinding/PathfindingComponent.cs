using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gulde.Entities;
using Gulde.Extensions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Pathfinding
{
    [ExecuteAlways]
    [RequireComponent(typeof(EntityComponent))]
    public class PathfindingComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        float Speed { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        float CellThreshold { get; set; }

        [OdinSerialize]
        [BoxGroup("Path")]
        Queue<Vector3Int> Waypoints { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [BoxGroup("Info")]
        EntityComponent EntityComponent { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        Vector3 Position => transform.position;

        [ShowInInspector]
        [BoxGroup("Info")]
        Vector3Int CellPosition => Position.ToCell();

        [ShowInInspector]
        [BoxGroup("Info")]
        bool HasWaypoints => Waypoints != null && Waypoints.Count > 0;

        bool IsAtWaypoint => HasWaypoints && Position.DistanceTo(CurrentWaypoint) < CellThreshold;

        Vector3Int CurrentWaypoint => Waypoints.Peek();

        public event EventHandler<CellEventArgs> DestinationReached;

        void Awake()
        {
            EntityComponent = GetComponent<EntityComponent>();
        }

        void FixedUpdate()
        {
            if (!HasWaypoints) return;

            var direction = Position.DirectionTo(CurrentWaypoint);

            transform.position += direction * (Speed * Time.fixedDeltaTime);

            if (!IsAtWaypoint) return;

            var cell = Waypoints.Dequeue();

            if (HasWaypoints) return;

            DestinationReached?.Invoke(this, new CellEventArgs(cell));
        }

        public void SetDestination(Vector3Int destinationCell)
        {
            Waypoints = Pathfinder.FindPath(CellPosition, destinationCell, EntityComponent.Map);
        }

        #region OdinInspector

        [OdinSerialize]
        [HideInInspector]
        bool IsSimulating { get; set; }

        [OdinSerialize]
        [HideInInspector]
        Coroutine Simulation { get; set; }

        string ButtonName => (IsSimulating && Simulation != null) ? "Stop Simulation" : "Start Simulation";

        IEnumerator SimulateFixedUpdate()
        {
            while (IsSimulating)
            {
                yield return new WaitForSeconds(Time.fixedDeltaTime);
                FixedUpdate();
            }
        }

        [Button(Name = "@ButtonName")]
        void ToggleSimulation()
        {
            if (IsSimulating && Simulation != null)
            {
                IsSimulating = false;
                StopCoroutine(Simulation);
                Simulation = null;
            }
            else
            {
                IsSimulating = true;
                Simulation = StartCoroutine(SimulateFixedUpdate());
            }
        }

        #endregion
    }
}