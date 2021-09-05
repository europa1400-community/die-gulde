using System;
using System.Collections.Generic;
using GuldeLib.Entities;
using GuldeLib.Entities.Pathfinding;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Maps
{
    [RequireComponent(typeof(NavComponent))]
    [RequireComponent(typeof(EntityRegistryComponent))]
    public class MapComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public Vector2Int Size { get; private set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<LocationComponent> Locations { get; } = new HashSet<LocationComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public EntityRegistryComponent EntityRegistry { get; private set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public NavComponent Nav { get; private set; }

        HashSet<EntityComponent> Entities => EntityRegistry.Entities;

        public event EventHandler<CellEventArgs> SizeChanged;
        public event EventHandler<LocationEventArgs> LocationRegistered;

        void Awake()
        {
            this.Log("Map initializing");

            Nav = GetComponent<NavComponent>();
            EntityRegistry = GetComponent<EntityRegistryComponent>();
            MapRegistry.Register(this);

            EntityRegistry.Registered += OnEntityRegistered;
            EntityRegistry.Unregistered += OnEntityUnregistered;

            SetSize(Size.x, Size.y);
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

        public void SetSize(int x, int y) => SetSize(new Vector2Int(x, y));

        public void SetSize(Vector2Int size)
        {
            this.Log($"Map setting size to {size}");

            Size = size;

            SizeChanged?.Invoke(this, new CellEventArgs((Vector3Int) Size));
        }

        void OnEntityRegistered(object sender, EntityEventArgs e)
        {
            this.Log($"Map registering {e.Entity}");

            e.Entity.SetMap(this);
        }

        void OnEntityUnregistered(object sender, EntityEventArgs e)
        {
            this.Log($"Map unregistering {e.Entity}");

            e.Entity.SetMap(null);
        }
    }
}