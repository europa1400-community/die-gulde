using System;
using UnityEngine;

namespace GuldeLib.Pathfinding
{
    public class CellEventArgs : EventArgs
    {
        public Vector2Int Cell { get; }

        public CellEventArgs(Vector2Int cell) => Cell = cell;
    }
}