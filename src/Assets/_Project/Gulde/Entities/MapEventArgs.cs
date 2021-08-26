using System;
using Gulde.Maps;

namespace Gulde.Entities
{
    public class MapEventArgs : EventArgs
    {
        public MapEventArgs(MapComponent map)
        {
            Map = map;
        }

        public MapComponent Map { get; }
    }
}