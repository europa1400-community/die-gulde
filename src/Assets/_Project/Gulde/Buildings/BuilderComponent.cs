using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

namespace Gulde.Buildings
{
    public class BuilderComponent : MonoBehaviour
    {
        [SerializeField] List<BuildSpace> _buildSpaces;
        [SerializeField] Building _selectedBuilding;

        [SerializeField] Tile _tileBuilding;

        [SerializeField] Canvas _canvasBuild;
        [SerializeField] Grid _gridCity;
        [SerializeField] Tilemap _mapBuildSpaces;
        [SerializeField] Tilemap _mapBuildings;
        [SerializeField] Tilemap _mapBuildingsHover;

        Camera Camera { get; set; }

        bool IsBuilding { get; set; }

        Vector2 MousePos => Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3Int MouseCellPosition => _gridCity.WorldToCell(MousePos);

        [SerializeField] InputAction _mouseRightClicked;
        [SerializeField] InputAction _mouseLeftClicked;

        void Start()
        {
            Camera = Camera.main;

            _buildSpaces = FindBuildSpaces(_mapBuildings);
        }

        void Update()
        {
            if (IsBuilding)
            {
                HoverBuilding(_selectedBuilding, MouseCellPosition, _mapBuildingsHover);
            }
        }

        public void OnButtonBuilding(Building building)
        {
            IsBuilding = true;
            _selectedBuilding = building;

            _mouseRightClicked.performed += OnMouseRightDown;
            _mouseLeftClicked.performed += OnMouseLeftDown;
        }

        void OnMouseRightDown(InputAction.CallbackContext context)
        {
            CancelBuilder(_mapBuildingsHover);

            _mouseRightClicked.performed -= OnMouseRightDown;
            _mouseLeftClicked.performed -= OnMouseLeftDown;
        }

        void OnMouseLeftDown(InputAction.CallbackContext context)
        {
            PlaceBuilding(_selectedBuilding, MouseCellPosition, _mapBuildings);
            CancelBuilder(_mapBuildingsHover);

            _mouseRightClicked.performed -= OnMouseRightDown;
            _mouseLeftClicked.performed -= OnMouseLeftDown;
        }

        void HoverBuilding(Building building, Vector3Int cellPosition, Tilemap tilemap)
        {
            tilemap.ClearAllTiles();

            PlaceBuilding(building, cellPosition, tilemap);
        }

        void CancelBuilder(Tilemap tilemap)
        {
            IsBuilding = false;
            tilemap.ClearAllTiles();
        }

        void PlaceBuilding(Building building, Vector3Int cellPosition, Tilemap tilemap)
        {
            foreach (var buildingCellPosition in building._cellPositions)
            {
                tilemap.SetTile(cellPosition + buildingCellPosition, _tileBuilding);
            }
        }

        List<BuildSpace> FindBuildSpaces(Tilemap mapBuildSpaces)
        {
            var buildSpaces = new List<BuildSpace>();

            foreach (var cellPosition in mapBuildSpaces.cellBounds.allPositionsWithin)
            {
                if (!mapBuildSpaces.HasTile(cellPosition)) continue;

                if (IsInBuildSpaces(cellPosition, buildSpaces)) continue;

                var buildSpace = GetBuildSpace(cellPosition, mapBuildSpaces);
                buildSpaces.Add(buildSpace);
            }

            return buildSpaces;
        }

        bool IsInBuildSpaces(Vector3Int cellPosition, List<BuildSpace> buildSpaces) => buildSpaces.Any(buildSpace => buildSpace.HasTile(cellPosition));

        BuildSpace GetBuildSpace(Vector3Int cellPosition, Tilemap mapBuildSpaces)
        {
            if (!mapBuildSpaces.HasTile(cellPosition)) return null;

            var cellPositions = new List<Vector3Int>();
            var nextPosition = cellPosition;
            var size = 0;

            while (mapBuildSpaces.HasTile(nextPosition))
            {
                size += 1;
                nextPosition += Vector3Int.right;
            }

            for (var x = cellPosition.x; x < cellPosition.x + size; x++)
            {
                for (var y = cellPosition.y; y < cellPosition.y + size; y++)
                {
                    var position = new Vector3Int(x, y, 0);
                    cellPositions.Add(position);
                }
            }

            var buildSpace = new BuildSpace(size, cellPositions);
            return buildSpace;
        }
    }
}
