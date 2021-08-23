using System;
using System.Collections.Generic;
using Gulde.Entities;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
        [ReadOnly]
        public NavComponent NavComponent { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        public EntityRegistryComponent EntityRegistry { get; private set; }

        [ShowInInspector]
        bool IsSelected => Locator.MapSelector && Locator.MapSelector.SelectedMap == this;

        HashSet<EntityComponent> Entities => EntityRegistry.Entities;

        void Awake()
        {
            NavComponent = GetComponent<NavComponent>();
            EntityRegistry = GetComponent<EntityRegistryComponent>();
            MapRegistry.Register(this);

            EntityRegistry.Registered += OnEntityRegistered;
            EntityRegistry.Unregistered += OnEntityUnregistered;
        }

        void OnEntityRegistered(object sender, EntityEventArgs e)
        {
            Debug.Log($"Map {name} registered entity {e.Entity.name}");
            var entityComponent = e.Entity;
            entityComponent.Map = this;

            SetEntityVisible(entityComponent, IsSelected);
        }

        void OnEntityUnregistered(object sender, EntityEventArgs e)
        {
            var entityComponent = e.Entity;
            entityComponent.Map = null;

            SetEntityVisible(entityComponent, false);
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