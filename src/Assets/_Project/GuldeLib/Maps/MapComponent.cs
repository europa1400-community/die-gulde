using System;
using System.Collections.Generic;
using GuldeLib.Entities;
using GuldeLib.Pathfinding;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Maps
{
    public class MapComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public Vector2Int Size { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public MapLayout MapLayout { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<LocationComponent> Locations { get; } = new HashSet<LocationComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public EntityRegistryComponent EntityRegistry => GetComponent<EntityRegistryComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public NavComponent Nav => GetComponent<NavComponent>();

        public event EventHandler<LocationEventArgs> LocationRegistered;

        public event EventHandler<MapComponentInitializedEventArgs> Initialized;

        void Awake()
        {
            this.Log("Map initializing");
        }

        void Start()
        {
            Initialized?.Invoke(this, new MapComponentInitializedEventArgs(Size, MapLayout));
        }

        public void Register(LocationComponent location)
        {
            this.Log($"Map registering {location}");

            location.SetContainingMap(this);
            location.EntitySpawned += OnEntitySpawned;

            Locations.Add(location);

            LocationRegistered?.Invoke(this, new LocationEventArgs(location));
        }

        void OnEntitySpawned(object sender, EntityEventArgs e)
        {
            this.Log($"Map spawning {e.Entity}");

            EntityRegistry.Register(e.Entity);
        }

        public void OnEntityRegistered(object sender, EntityEventArgs e)
        {
            this.Log($"Map registering {e.Entity}");

            e.Entity.SetMap(this);
        }

        public void OnEntityUnregistered(object sender, EntityEventArgs e)
        {
            this.Log($"Map unregistering {e.Entity}");

            e.Entity.SetMap(null);
        }
    }
}