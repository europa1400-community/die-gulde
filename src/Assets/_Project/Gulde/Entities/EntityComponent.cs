using System;
using Gulde.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

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

        public event EventHandler<MapEventArgs> MapChanged;
        public event EventHandler<LocationEventArgs> LocationChanged;

        void Awake()
        {
            Renderer = GetComponent<EntityRendererComponent>();
        }

        public void SetLocation(LocationComponent location)
        {
            Location = location;

            LocationChanged?.Invoke(this, new LocationEventArgs(location));
        }

        public void SetMap(MapComponent map)
        {
            Map = map;

            MapChanged?.Invoke(this, new MapEventArgs(map));
        }
    }
}