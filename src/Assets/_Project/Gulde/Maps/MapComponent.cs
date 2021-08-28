using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Company;
using Gulde.Company.Employees;
using Gulde.Economy;
using Gulde.Entities;
using Gulde.Extensions;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Gulde.Maps
{
    [RequireComponent(typeof(NavComponent))]
    [RequireComponent(typeof(EntityRegistryComponent))]
    public class MapComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        Grid Grid { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        public Vector2Int Size { get; private set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<LocationComponent> Locations { get; } = new HashSet<LocationComponent>();

        [ShowInInspector]
        [BoxGroup("Info")]
        bool IsSelected => Locator.MapSelector && Locator.MapSelector.SelectedMap == this;

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public EntityRegistryComponent EntityRegistry { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public NavComponent Nav { get; private set; }

        HashSet<EntityComponent> Entities => EntityRegistry.Entities;

        public event EventHandler<CellEventArgs> SizeChanged;
        public event EventHandler<LocationEventArgs> LocationRegistered;

        void Awake()
        {
            Nav = GetComponent<NavComponent>();
            EntityRegistry = GetComponent<EntityRegistryComponent>();
            MapRegistry.Register(this);

            EntityRegistry.Registered += OnEntityRegistered;
            EntityRegistry.Unregistered += OnEntityUnregistered;

            SetSize(Size.x, Size.y);
        }

        public void Register(LocationComponent location)
        {
            location.SetContainingMap(this);
            location.EntitySpawned += OnEntitySpawned;

            Locations.Add(location);

            LocationRegistered?.Invoke(this, new LocationEventArgs(location));
        }

        void OnEntitySpawned(object sender, EntityEventArgs e)
        {
            EntityRegistry.Register(e.Entity);
        }

        public void SetSize(int x, int y)
        {
            Size = new Vector2Int(x, y);

            SizeChanged?.Invoke(this, new CellEventArgs((Vector3Int) Size));
        }

        void OnEntityRegistered(object sender, EntityEventArgs e)
        {
            e.Entity.SetMap(this);

            SetEntityVisible(e.Entity, IsSelected);
        }

        void OnEntityUnregistered(object sender, EntityEventArgs e)
        {
            e.Entity.SetMap(null);

            SetEntityVisible(e.Entity, false);
        }

        public void SetVisible(bool isVisible)
        {
            SetGridVisible(isVisible);

            foreach (var entity in Entities)
            {
                if (!entity) continue;
                SetEntityVisible(entity, isVisible);
            }
        }

        void SetGridVisible(bool isVisible)
        {
            if (!Grid) return;
            Grid.gameObject.SetActive(isVisible);
        }

        void SetEntityVisible(EntityComponent entity, bool isVisible)
        {
            var entityRenderer = entity.Renderer;
            if (!entityRenderer) return;
            entityRenderer.SetVisible(isVisible);
        }
    }
}