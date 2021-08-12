using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Gulde.Buildings
{
    [CreateAssetMenu(menuName = "Building")]
    public class BuildingLayout : ScriptableObject
    {
        [SerializeField] public Dictionary<Vector3Int, TileBase> _tiles = new Dictionary<Vector3Int, TileBase>();
        [SerializeField] public Dictionary<Vector3Int, TileBase> _tilesTraversable = new Dictionary<Vector3Int, TileBase>();
        [SerializeField] public Vector3Int _entrancePosition;
        [SerializeField] public BuildingType _buildingType;
        [SerializeField] public GameObject _mapPrefab;
    }

    public static class BuildingLayoutExtensions
    {
        public static BuildingLayout Orientate(this BuildingLayout buildingLayout, Orientation orientation)
        {
            var newLayout = ScriptableObject.CreateInstance<BuildingLayout>();
            var newTiles = new Dictionary<Vector3Int, TileBase>();
            var newTilesTraversable = new Dictionary<Vector3Int, TileBase>();

            foreach (var pair in buildingLayout._tiles) 
            {
                var transformedCellPosition = orientation switch
                {
                    Orientation.Up => new Vector3Int(-pair.Key.y, pair.Key.x, 0),
                    Orientation.Down => new Vector3Int(pair.Key.y, -pair.Key.x, 0),
                    Orientation.Left => new Vector3Int(-pair.Key.x, -pair.Key.y, 0),
                    Orientation.Right => pair.Key,
                    _ => pair.Key,
                };

                newTiles.Add(transformedCellPosition, pair.Value);
            }
            
            foreach (var pair in buildingLayout._tilesTraversable) 
            {
                var transformedCellPosition = orientation switch
                {
                    Orientation.Up => new Vector3Int(-pair.Key.y, pair.Key.x, 0),
                    Orientation.Down => new Vector3Int(pair.Key.y, -pair.Key.x, 0),
                    Orientation.Left => new Vector3Int(-pair.Key.x, -pair.Key.y, 0),
                    Orientation.Right => pair.Key,
                    _ => pair.Key,
                };

                newTilesTraversable.Add(transformedCellPosition, pair.Value);
            }

            var transformedEntrancePosition = orientation switch
            {
                Orientation.Up => new Vector3Int(-buildingLayout._entrancePosition.y, buildingLayout._entrancePosition.x, 0),
                Orientation.Down => new Vector3Int(buildingLayout._entrancePosition.y, -buildingLayout._entrancePosition.x, 0),
                Orientation.Left => new Vector3Int(-buildingLayout._entrancePosition.x, -buildingLayout._entrancePosition.y, 0),
                Orientation.Right => buildingLayout._entrancePosition,
                _ => buildingLayout._entrancePosition,
            };

            newLayout._entrancePosition = transformedEntrancePosition;
            newLayout._tiles = newTiles;
            newLayout._buildingType = buildingLayout._buildingType;
            
            newLayout._tilesTraversable = newTilesTraversable;

            return newLayout;
        }
    }
}