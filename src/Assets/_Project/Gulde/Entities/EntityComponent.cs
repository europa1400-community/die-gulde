using System;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Gulde.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Entities
{
    public class EntityComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public LocationComponent Location { get; private set; }

        [OdinSerialize]
        public MapComponent Map { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        public EntityRendererComponent Renderer { get; private set; }

        public Vector3Int CellPosition => transform.position.ToCell();

        public event EventHandler<MapEventArgs> MapChanged;
        public event EventHandler<LocationEventArgs> LocationChanged;

        void Awake()
        {
            this.Log("Entity initializing");

            Renderer = GetComponent<EntityRendererComponent>();
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

        public void SetCell(Vector3Int cell)
        {
            this.Log($"Entity setting cell to {cell}");

            transform.position = cell.ToWorld();
        }
    }
}