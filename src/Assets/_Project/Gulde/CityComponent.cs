using System.Collections.Generic;
using Gulde.Buildings;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gulde
{
    public class CityComponent : MonoBehaviour
    {
        [SerializeField] Tilemap _mapBackground;
        [SerializeField] Tilemap _mapBuildSpaces;
        [SerializeField] List<BuildingComponent> _buildingComponents = new List<BuildingComponent>();

        BuilderComponent BuilderComponent { get; set; }

        public List<Vector3Int> Cells { get; private set; }

        void Awake()
        {
            Cells = GetCells();
        }

        void Start()
        {
            BuilderComponent = GetComponent<BuilderComponent>();
            BuilderComponent.BuildingCreated += OnBuildingCreated;
        }

        void OnBuildingCreated(BuildingComponent buildingComponent)
        {
            _buildingComponents.Add(buildingComponent);
        }

        List<Vector3Int> GetCells()
        {
            var cells = new List<Vector3Int>();
            foreach (var cell in _mapBackground.cellBounds.allPositionsWithin)
            {
                if (_mapBuildSpaces.HasTile(cell)) continue;
                cells.Add(cell);
            }
            return cells;
        }

        public Vector3Int WorldToCell(Vector3 position) => _mapBackground.WorldToCell(position);
    }
}