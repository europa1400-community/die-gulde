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
        Vector2 _position;

        [ShowInInspector]
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                PositionChanged?.Invoke(this, new PositionChangedEventArgs(value));
            } }

        [ShowInInspector]
        public LocationComponent Location { get; private set; }

        [ShowInInspector]
        public MapComponent Map { get; private set; }

        public Vector2Int CellPosition => Position.ToCell();

        public event EventHandler<MapEventArgs> MapChanged;
        public event EventHandler<LocationEventArgs> LocationChanged;
        public event EventHandler<EntityComponentInitializedEventArgs> Initialized;
        public event EventHandler<PositionChangedEventArgs> PositionChanged;

        void Awake()
        {
            this.Log("Entity initializing");
        }

        void Start()
        {
            Initialized?.Invoke(this, new EntityComponentInitializedEventArgs(Position, Location, Map));
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