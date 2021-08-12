using System.Collections;
using System.Collections.Generic;
using Gulde.Buildings;
using Gulde.Extensions;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Population
{
    public class EntityComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public LocationComponent Location { get; set; }

        [ShowInInspector]
        [ReadOnly]
        Vector3Int StartCell { get; set; }

        [ShowInInspector]
        [ReadOnly]
        Vector3Int EndCell { get; set; }

        [ShowInInspector]
        [ReadOnly]
        float TravelSpeed { get; set; }

        [ShowInInspector]
        [ReadOnly]
        float TravelThreshold { get; set; }

        [ShowInInspector]
        [ReadOnly]
        Vector3Int CurrentCell { get; set; }

        [ShowInInspector]
        [ReadOnly]
        Vector3 CurrentWaypoint { get; set; }

        [ShowInInspector]
        [ReadOnly]
        Vector3 CurrentDirection { get; set; }

        [ShowInInspector]
        [ReadOnly]
        List<Vector3Int> CurrentDirections { get; set; }

        [ShowInInspector]
        [ReadOnly]
        List<Vector3Int> CurrentWaypoints { get; set; }

        SpriteRenderer SpriteRenderer { get; set; }

        // void Awake()
        // {
        //     SpriteRenderer = GetComponent<SpriteRenderer>();
        // }
        //
        // void Start()
        // {
        //     SetCell(StartCell);
        //     TravelTo(EndCell);
        // }
        //
        // void Update()
        // {
        //     CurrentCell = transform.position.ToCell();
        //
        //     if ((transform.position - CurrentWaypoint).magnitude <= TravelThreshold)
        //     {
        //         if (CurrentWaypoints.Count == 0)
        //         {
        //             CurrentWaypoint = CurrentCell;
        //             CurrentDirection = Vector3.zero;
        //         }
        //         else
        //         {
        //             PollWaypoint();
        //             PollDirection();
        //         }
        //     }
        //
        //     if (CurrentDirection != Vector3.zero)
        //     {
        //         Debug.DrawLine(CurrentCell, CurrentWaypoint, Color.blue);
        //     }
        //
        //     transform.position += ((Vector3)CurrentDirection).normalized * (TravelSpeed * Time.deltaTime);
        // }
        //
        // void SetCell(Vector3Int cell)
        // {
        //     CurrentCell = cell;
        //     transform.position = new Vector3(cell.x + 0.5f, cell.y + 0.5f);
        // }
        //
        // void TravelTo(Vector3Int destinationCell)
        // {
        //     CurrentWaypoints = Pathfinder.FindPath(CurrentCell, destinationCell);
        //     CurrentDirections = Pathfinder.GetDirections(CurrentCell, CurrentWaypoints);
        //     PollWaypoint();
        //     PollDirection();
        // }
        //
        // void PollWaypoint()
        // {
        //     CurrentWaypoint = CurrentWaypoints[0];
        //     CurrentWaypoint += 0.5f * new Vector3(1f, 1f);
        //     CurrentWaypoints.RemoveAt(0);
        // }
        //
        // void PollDirection()
        // {
        //     CurrentDirection = CurrentDirections[0];
        //     CurrentDirection = CurrentDirection.normalized;
        //     CurrentDirections.RemoveAt(0);
        // }
    }
}