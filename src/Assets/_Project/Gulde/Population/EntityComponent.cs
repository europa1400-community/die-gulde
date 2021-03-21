using System.Collections;
using System.Collections.Generic;
using Gulde.Buildings;
using Gulde.Pathfinding;
using UnityEngine;

namespace Gulde.Population
{
    public class EntityComponent : MonoBehaviour
    {
        [SerializeField] CityComponent _cityComponent;
        [SerializeField] Vector3Int _startCell;
        [SerializeField] Vector3Int _endCell;
        [SerializeField] float _travelSpeed;
        [SerializeField] float _travelThreshold;

        SpriteRenderer SpriteRenderer { get; set; }
        Transform Transform { get; set; }

        Vector3Int CurrentCell { get; set; }
        Vector3 CurrentWaypoint { get; set; }
        Vector3 CurrentDirection { get; set; }
        List<Vector3Int> CurrentDirections { get; set; }
        List<Vector3Int> CurrentWaypoints { get; set; }

        void Start()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Transform = transform;

            SetCell(_startCell);
            TravelTo(_endCell);
        }

        void Update()
        {
            CurrentCell = _cityComponent.WorldToCell(transform.position);

            if ((transform.position - CurrentWaypoint).magnitude <= _travelThreshold)
            {
                if (CurrentWaypoints.Count == 0)
                {
                    Debug.Log("Reached Destination");
                    CurrentWaypoint = CurrentCell;
                    CurrentDirection = Vector3.zero;
                }
                else
                {
                    Debug.Log("Reached Waypoint");
                    PollWaypoint();
                    PollDirection();
                }
            }

            if (CurrentDirection != Vector3.zero)
            {
                Debug.DrawLine(CurrentCell, CurrentWaypoint, Color.blue);
            }

            transform.position += ((Vector3)CurrentDirection).normalized * (_travelSpeed * Time.deltaTime);
        }

        void SetCell(Vector3Int cell)
        {
            CurrentCell = cell;
            Transform.position = new Vector3(cell.x + 0.5f, cell.y + 0.5f);
        }

        void TravelTo(Vector3Int destinationCell)
        {
            CurrentWaypoints = Pathfinder.FindPath(CurrentCell, destinationCell, _cityComponent.Cells);
            CurrentDirections = Pathfinder.GetDirections(CurrentCell, CurrentWaypoints);
            PollWaypoint();
            PollDirection();
        }

        void PollWaypoint()
        {
            CurrentWaypoint = CurrentWaypoints[0];
            CurrentWaypoint += 0.5f * new Vector3(1f, 1f);
            CurrentWaypoints.RemoveAt(0);
        }

        void PollDirection()
        {
            CurrentDirection = CurrentDirections[0];
            CurrentDirection = CurrentDirection.normalized;
            CurrentDirections.RemoveAt(0);
        }
    }
}