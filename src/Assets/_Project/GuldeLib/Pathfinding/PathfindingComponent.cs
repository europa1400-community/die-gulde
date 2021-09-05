using System;
using System.Collections.Generic;
using GuldeLib.Entities;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Pathfinding
{
    [RequireComponent(typeof(EntityComponent))]
    public class PathfindingComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        float Speed { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        float CellThreshold { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public Queue<Vector3Int> Waypoints { get; private set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        Vector3Int CellPosition => Position.ToCell();

        [ShowInInspector]
        [BoxGroup("Info")]
        bool HasWaypoints => Waypoints != null && Waypoints.Count > 0;

        [ShowInInspector]
        [BoxGroup("Info")]
        public float TravelPercentage => RemainingWaypoints / TotalWaypoints;

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        EntityComponent EntityComponent { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        int TotalWaypoints { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        int RemainingWaypoints => Waypoints.Count;

        Vector3 Position => transform.position;

        bool IsAtWaypoint => HasWaypoints && Position.DistanceTo(CurrentWaypoint) < CellThreshold;

        Vector3Int CurrentWaypoint => Waypoints.Peek();
        public WaitForDestinationReached WaitForDestinationReached => new WaitForDestinationReached(this);
        public WaitForDestinationReachedPartly WaitForDestinationReachedPartly(float percentage) => new WaitForDestinationReachedPartly(this, percentage);

        public event EventHandler<CellEventArgs> DestinationChanged;
        public event EventHandler<CellEventArgs> DestinationReached;

        void Awake()
        {
            this.Log("Pathfinding initializing");

            EntityComponent = GetComponent<EntityComponent>();
        }

        void FixedUpdate()
        {
            if (!HasWaypoints) return;

            var direction = Position.DirectionTo(CurrentWaypoint);

            transform.position += direction * (Speed * Time.fixedDeltaTime);

            if (!IsAtWaypoint) return;

            this.Log($"Pathfinding reached waypoint {CurrentWaypoint}");

            var cell = Waypoints.Dequeue();

            if (HasWaypoints) return;

            this.Log($"Pathfinding reached destination");

            DestinationReached?.Invoke(this, new CellEventArgs(cell));
        }

        public void SetDestination(Vector3Int destinationCell)
        {
            if (!Application.isPlaying)
            {
                this.Log($"Pathfinding relocating entity to {destinationCell}");

                transform.position = destinationCell.ToWorld();
                DestinationReached?.Invoke(this, new CellEventArgs(destinationCell));
                return;
            }

            this.Log($"Pathfinding sending entity to {destinationCell}");

            if (CellPosition == destinationCell)
            {
                this.Log($"Pathfinding entity was already at {destinationCell}");

                DestinationReached?.Invoke(this, new CellEventArgs(destinationCell));
                return;
            }

            Waypoints = Pathfinder.FindPath(CellPosition, destinationCell, EntityComponent.Map);

            if (Waypoints == null || Waypoints.Count == 0)
            {
                this.Log($"Pathfinding couldn't find a path!", LogType.Warning);

                DestinationReached?.Invoke(this, new CellEventArgs(destinationCell));
            }
            else TotalWaypoints = Waypoints.Count;
        }
    }

    public class WaitForDestinationReached : CustomYieldInstruction
    {
        public WaitForDestinationReached(PathfindingComponent pathfinding)
        {
            Pathfinding = pathfinding;
            Pathfinding.DestinationReached += OnDestinationReached;
        }

        void OnDestinationReached(object sender, CellEventArgs e)
        {
            IsDestinationReached = true;
        }

        PathfindingComponent Pathfinding { get; }
        bool IsDestinationReached { get; set; }
        public override bool keepWaiting => !IsDestinationReached && Pathfinding.Waypoints.Count != 0;
    }

    public class WaitForDestinationReachedPartly : CustomYieldInstruction
    {
        public WaitForDestinationReachedPartly(PathfindingComponent pathfinding, float percentage)
        {
            Pathfinding = pathfinding;
            Percentage = percentage;
        }
        PathfindingComponent Pathfinding { get; }
        float Percentage { get; }
        public override bool keepWaiting => Pathfinding.TravelPercentage < Percentage;
    }
}