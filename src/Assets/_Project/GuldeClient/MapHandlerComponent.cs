using GuldeLib.Extensions;
using GuldeLib.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GuldeClient
{
    public class MapHandlerComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        Tilemap BackgroundTilemap { get; set; }

        [OdinSerialize]
        Tilemap BuildSpaceTilemap { get; set; }

        [OdinSerialize]
        Tilemap BuildingTilemap { get; set; }

        [OdinSerialize]
        TileBase BackgroundTile { get; set; }

        [OdinSerialize]
        TileBase BuildSpaceTile { get; set; }

        [OdinSerialize]
        TileBase BuildingTile { get; set; }

        [OdinSerialize]
        TileBase BuildingEntryCellTile { get; set; }

        public void OnInitialized(object sender, MapComponent.InitializedEventArgs e)
        {
            for (var x = -e.Size.x / 2; x < e.Size.x / 2; x++)
            {
                for (var y = -e.Size.y / 2; y < e.Size.y / 2; y++)
                {
                    var cell = new Vector3Int(x, y, 0);
                    BackgroundTilemap.SetTile(cell, BackgroundTile);
                }
            }

            foreach (var pair in e.MapLayout.CellToBuildSpace)
            {
                var centerCell = pair.Key;
                var buildSpace = pair.Value;

                foreach (var cell in centerCell.GetCellsWithin(buildSpace.Size))
                {
                    BuildSpaceTilemap.SetTile((Vector3Int)cell, BuildSpaceTile);
                }
            }

            foreach (var pair in e.MapLayout.CellToBuilding)
            {
                var buildSpaceCell = pair.Key;
                var building = pair.Value.Item1;
                var entryCell = pair.Value.Item2;

                foreach (var cell in buildSpaceCell.GetCellsWithin(building.Size))
                {
                    BuildingTilemap.SetTile((Vector3Int) cell, BuildingTile);
                }
                BuildingTilemap.SetTile((Vector3Int)entryCell, BuildingEntryCellTile);
            }
        }
    }
}