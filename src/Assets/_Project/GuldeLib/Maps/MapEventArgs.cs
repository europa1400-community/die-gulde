using System;

namespace GuldeLib.Maps
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