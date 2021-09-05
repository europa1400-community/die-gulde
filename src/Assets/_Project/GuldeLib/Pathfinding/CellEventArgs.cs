using System;
using UnityEngine;

namespace GuldeLib.Pathfinding
{
    public class CellEventArgs : EventArgs
    {
        public Vector3Int Cell { get; }

        public CellEventArgs(Vector3Int cell) => Cell = cell;
    }
}