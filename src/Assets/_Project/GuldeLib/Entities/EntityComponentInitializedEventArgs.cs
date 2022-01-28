using System;
using GuldeLib.Maps;
using UnityEngine;

namespace GuldeLib.Entities
{
    public class EntityComponentInitializedEventArgs : EventArgs
    {
        public Vector2 Position { get; set; }
        
        public LocationComponent Location { get; private set; }
        
        public MapComponent Map { get; private set; }

        public EntityComponentInitializedEventArgs(Vector2 position, LocationComponent location, MapComponent map)
        {
            Position = position;
            Location = location;
            Map = map;
        }
    }
}