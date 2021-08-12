using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

namespace Gulde.Buildings
{
    public class BuilderComponent : MonoBehaviour
    {
        [SerializeField] List<BuildSpace> _buildSpaces;
        [SerializeField] BuildingLayout _selectedBuildingLayout;

        [SerializeField] Tile _tileBuilding;
        [SerializeField] Tile _tileEntrance;

        [SerializeField] Canvas _canvasBuild;
        [SerializeField] Grid _gridCity;
        [SerializeField] Tilemap _mapBuildSpaces;
        [SerializeField] Tilemap _mapBuildings;
        [SerializeField] Tilemap _mapBuildingsHover;

        Camera Camera { get; set; }

        bool IsBuilding { get; set; }
        Orientation BuildOrientation { get; set; }

        Vector2 MousePos => Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3Int MouseCellPosition => _gridCity.WorldToCell(MousePos);

        [SerializeField] InputAction _mouseRightClicked;
        [SerializeField] InputAction _mouseLeftClicked;
        [SerializeField] InputAction _mouseWheelChanged;

        public event Action<BuildingComponent> BuildingCreated;

        void Start()
        {
            Camera = Camera.main;

            _buildSpaces = FindBuildSpaces(_mapBuildSpaces);

            _mouseRightClicked.performed += OnMouseRightDown;
            _mouseLeftClicked.performed += OnMouseLeftDown;
            _mouseWheelChanged.performed += OnMouseWheelChanged;
        }

        void Update()
        {
            if (IsBuilding)
            {
                HoverBuilding(_selectedBuildingLayout, MouseCellPosition, BuildOrientation, _mapBuildingsHover);
            }
        }

        public void OnButtonBuilding(BuildingLayout buildingLayout)
        {
            IsBuilding = true;
            BuildOrientation = Orientation.Right;
            _selectedBuildingLayout = buildingLayout;

            _mouseRightClicked.Enable();
            _mouseLeftClicked.Enable();
            _mouseWheelChanged.Enable();
        }

        void OnMouseRightDown(InputAction.CallbackContext context)
        {
            CancelBuilder(_mapBuildingsHover);
        }

        void OnMouseLeftDown(InputAction.CallbackContext context)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            PlaceBuilding(_selectedBuildingLayout, MouseCellPosition, BuildOrientation, _mapBuildings, _buildSpaces);
        }

        void OnMouseWheelChanged(InputAction.CallbackContext context)
        {
            var axis = context.ReadValue<float>();

            if (axis > 0) BuildOrientation = (Orientation)(((int)BuildOrientation + 1) % 4);
            else if (axis < 0) BuildOrientation = (Orientation)(((int) BuildOrientation - 1) % 4);

            if ((int)BuildOrientation < 0) BuildOrientation = (Orientation)3;
        }

        void HoverBuilding(BuildingLayout buildingLayout, Vector3Int cellPosition, Orientation orientation, Tilemap tilemap)
        {
            tilemap.ClearAllTiles();

            foreach (var buildingCellPosition in buildingLayout._tiles.Keys)
            {
                var transformedCellPosition = orientation switch
                {
                    Orientation.Up => new Vector3Int(buildingCellPosition.y, buildingCellPosition.x, 0),
                    Orientation.Down => new Vector3Int(buildingCellPosition.y, -buildingCellPosition.x, 0),
                    Orientation.Left => new Vector3Int(-buildingCellPosition.x, buildingCellPosition.y, 0),
                    Orientation.Right => buildingCellPosition,
                    _ => buildingCellPosition,
                };

                var isEntrance = buildingLayout._entrancePosition == buildingCellPosition;
                var tile = isEntrance ? _tileEntrance : _tileBuilding;
                tilemap.SetTile(cellPosition + transformedCellPosition, tile);
            }
        }

        void CancelBuilder(Tilemap tilemap)
        {
            IsBuilding = false;
            tilemap.ClearAllTiles();

            _mouseRightClicked.Disable();
            _mouseLeftClicked.Disable();
            _mouseWheelChanged.Disable();
        }

        void PlaceBuilding(BuildingLayout buildingLayout, Vector3Int cellPosition, Orientation orientation, Tilemap tilemap,
            List<BuildSpace> buildSpaces)
        {
            if (!CanPlace(buildingLayout, cellPosition, orientation, tilemap, buildSpaces))
            {
                return;
            }

            foreach (var buildingCellPosition in buildingLayout._tiles.Keys)
            {
                var transformedCellPosition = orientation switch
                {
                    Orientation.Up => new Vector3Int(buildingCellPosition.y, buildingCellPosition.x, 0),
                    Orientation.Down => new Vector3Int(buildingCellPosition.y, -buildingCellPosition.x, 0),
                    Orientation.Left => new Vector3Int(-buildingCellPosition.x, buildingCellPosition.y, 0),
                    Orientation.Right => buildingCellPosition,
                    _ => buildingCellPosition,
                };

                tilemap.SetTile(cellPosition + transformedCellPosition, _tileBuilding);
            }

            CreateBuilding(buildingLayout, cellPosition, orientation);

            CancelBuilder(_mapBuildingsHover);
        }

        BuildingComponent CreateBuilding(BuildingLayout buildingLayout, Vector3Int cellPosition,
            Orientation orientation)
        {
            var building = Instantiate(Prefab.BuildingPrefab, cellPosition, orientation.ToQuaternion());
            var buildingComponent = building.GetComponent<BuildingComponent>();

            buildingComponent._buildingLayout = buildingLayout;
            buildingComponent.Position = cellPosition;
            buildingComponent.Orientation = orientation;

            BuildingCreated?.Invoke(buildingComponent);

            return buildingComponent;
        }

        bool CanPlace(BuildingLayout buildingLayout, Vector3Int cellPosition, Orientation orientation, Tilemap tilemap, List<BuildSpace> buildSpaces)
        {
            foreach (var buildingCellPosition in buildingLayout._tiles.Keys)
            {
                var transformedCellPosition = orientation switch
                {
                    Orientation.Up => new Vector3Int(buildingCellPosition.y, buildingCellPosition.x, 0),
                    Orientation.Down => new Vector3Int(buildingCellPosition.y, -buildingCellPosition.x, 0),
                    Orientation.Left => new Vector3Int(-buildingCellPosition.x, buildingCellPosition.y, 0),
                    Orientation.Right => buildingCellPosition,
                    _ => buildingCellPosition,
                };

                if (tilemap.HasTile(cellPosition + transformedCellPosition)) return false;

                var isOnBuildSpace = false;

                foreach (var buildSpace in buildSpaces)
                {
                    if (buildSpace.HasTile(cellPosition + transformedCellPosition))
                    {
                        isOnBuildSpace = true;
                        break;
                    }
                }

                if (!isOnBuildSpace) return false;
            }

            return true;
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
