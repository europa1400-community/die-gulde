using System;
using System.Collections.Generic;
using Gulde.Entities;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Maps
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(NavComponent))]
    public class MapComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        Grid Grid { get; set; }

        [OdinSerialize]
        [ListDrawerSettings(Expanded = true)]
        HashSet<EntityComponent> Entities { get; set; } = new HashSet<EntityComponent>();

        [OdinSerialize]
        [ReadOnly]
        public NavComponent NavComponent { get; private set; }

        public event EventHandler<EntityEventArgs> EntityRegistered;
        public event EventHandler<EntityEventArgs> EntityUnregistered;

        public bool IsEntityRegistered(EntityComponent entityComponent) => Entities.Contains(entityComponent);

        [ShowInInspector]
        bool IsSelected => Locator.MapSelectorComponent && Locator.MapSelectorComponent.SelectedMap == this;

        void Awake()
        {
            NavComponent = GetComponent<NavComponent>();
            MapRegistry.RegisterMap(this);
        }

        public void RegisterEntity(EntityComponent entityComponent)
        {
            if (!entityComponent) return;

            Entities.Add(entityComponent);

            entityComponent.Map = this;

            EntityRegistered?.Invoke(this, new EntityEventArgs(entityComponent));

            SetEntityVisible(entityComponent, IsSelected);
        }

        public void UnregisterEntity(EntityComponent entityComponent)
        {
            if (!entityComponent) return;

            Entities.Remove(entityComponent);

            entityComponent.Map = null;

            EntityUnregistered?.Invoke(this, new EntityEventArgs(entityComponent));

            SetEntityVisible(entityComponent, false);
        }

        public void SetVisible(bool isVisible)
        {
            SetGridVisible(isVisible);

            foreach (var entity in Entities)
            {
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
            var entityRenderer = entity.GetComponent<EntityRendererComponent>();
            if (!entityRenderer) return;
            entityRenderer.SetVisible(isVisible);
        }

        #region OdinInspector

        void OnValidate()
        {
            NavComponent = GetComponent<NavComponent>();
            MapRegistry.RegisterMap(this);
        }

        #endregion
    }
}