using System.Collections.Generic;
using System.Linq;
using Gulde.Extensions;
using Gulde.Population;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Pathfinding
{
    [RequireComponent(typeof(EntityComponent))]
    public class PathfindingComponent : MonoBehaviour
    {
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
        bool HasWaypoints => Waypoints.Count > 0;

        bool IsAtWaypoint => HasWaypoints && Position.DistanceTo(Waypoints.Peek()) < CellThreshold;

        void Awake()
        {
            EntityComponent = GetComponent<EntityComponent>();
        }

        void FixedUpdate()
        {
            if (IsAtWaypoint) Waypoints.Dequeue();
        }

        void SetDestination(Vector3Int destinationCell)
        {
            Waypoints = Pathfinder.FindPath(CellPosition, destinationCell, EntityComponent.Map);
        }
    }
}