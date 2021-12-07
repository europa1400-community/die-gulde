using System;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Extensions;
using GuldeLib.Maps;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Generators
{
    public class GeneratableMapLayout : Generatable<MapLayout>
    {
        protected override bool SupportsDefaultGeneration => false;

        public override void Generate()
        {

        }

        public void Generate(Map map)
        {
            Value ??= new MapLayout();

            this.Log($"MapLayout data generating.");

            foreach (var buildSpaceType in map.BuildSpacePriorities)
            {
                var buildSpaces = map.BuildSpaces.Where(e => e.Type == buildSpaceType).ToList();

                foreach (var buildSpace in buildSpaces)
                {
                    for (var i = 0; i < buildSpace.Count; i++)
                    {
                        var cell = FindValidCell(map, buildSpace.Size);
                        if (!cell.HasValue) continue;

                        Value.CellToBuildSpace.Add(cell.Value, buildSpace);
                    }
                }
            }

            this.Log($"MapLayout data generated.");
        }

        Vector2Int? FindValidCell(Map map, Vector2Int size)
        {
            var candidateCell = Vector2Int.zero;
            var maxRadius = Mathf.Max(map.Size.Value.x, map.Size.Value.y);

            if (IsValidCell(candidateCell, size, map.Spacing)) return candidateCell;

            for (var radius = 1; radius < maxRadius; radius++)
            {
                for (var angle = 0f; angle < 360; angle += 0.5f / radius)
                {
                    if (IsValidCell(candidateCell, size, map.Spacing)) return candidateCell;
                }
            }

            this.Log("Could not find valid cell.", LogType.Warning);
            return null;
        }

        bool IsValidCell(Vector2Int candidateCell, Vector2Int size, int spacing)
        {
            var overlapSize = new Vector2Int(size.x + 2 * spacing, size.y + 2 * spacing);
            var cells = candidateCell.GetCellsWithin(overlapSize);

            var otherCells = new List<Vector2Int>();
            foreach (var pair in Value.CellToBuildSpace)
            {
                otherCells.AddRange(pair.Key.GetCellsWithin(pair.Value.Size));
            }

            foreach (var cell in cells)
            {
                if (!otherCells.Contains(cell)) continue;
                return false;
            }

            return true;
        }
    }
}