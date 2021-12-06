using System;
using GuldeLib.Extensions;
using GuldeLib.Maps;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Entities
{
    public class EntityComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        public Vector2 Position { get; set; }

        [ShowInInspector]
        public LocationComponent Location { get; private set; }

        [ShowInInspector]
        public MapComponent Map { get; private set; }

        public Vector2Int CellPosition => Position.ToCell();

        public event EventHandler<MapEventArgs> MapChanged;
        public event EventHandler<LocationEventArgs> LocationChanged;

        void Awake()
        {
            this.Log("Entity initializing");
        }

        public void SetLocation(LocationComponent location)
        {
            this.Log($"Entity setting location to {location}");

            Location = location;

            LocationChanged?.Invoke(this, new LocationEventArgs(location));
        }

        public void SetMap(MapComponent map)
        {
            this.Log($"Entity setting map to {map}");

            Map = map;

            MapChanged?.Invoke(this, new MapEventArgs(map));
        }

        public void SetCell(Vector2Int cell)
        {
            this.Log($"Entity setting cell to {cell}");

            Position = cell.ToWorld();
        }
    }
}