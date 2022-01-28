using System;
using UnityEngine;

namespace GuldeLib.Maps
{
    public class MapComponentInitializedEventArgs : EventArgs
    {
        public Vector2Int Size { get; }

        public MapLayout MapLayout { get; }

        public MapComponentInitializedEventArgs(Vector2Int size, MapLayout mapLayout)
        {
            Size = size;
            MapLayout = mapLayout;
        }
    }
}