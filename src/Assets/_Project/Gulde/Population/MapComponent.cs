using System;
using System.Collections.Generic;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Population
{
    [RequireComponent(typeof(NavComponent))]
    public class MapComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ListDrawerSettings(Expanded = true)]
        HashSet<EntityComponent> Entities { get; set; } = new HashSet<EntityComponent>();

        [OdinSerialize]
        [ReadOnly]
        public NavComponent NavComponent { get; private set; }

        public event EventHandler<EntityEventArgs> EntityRegistered;
        public event EventHandler<EntityEventArgs> EntityUnregistered;

        public bool IsEntityRegistered(EntityComponent entityComponent) => Entities.Contains(entityComponent);

        void Awake()
        {
            NavComponent = GetComponent<NavComponent>();
        }

        public void RegisterEntity(EntityComponent entityComponent)
        {
            if (!entityComponent) return;

            Entities.Add(entityComponent);

            entityComponent.Map = this;

            EntityRegistered?.Invoke(this, new EntityEventArgs(entityComponent));
        }

        public void UnregisterEntity(EntityComponent entityComponent)
        {
            if (!entityComponent) return;

            Entities.Remove(entityComponent);

            entityComponent.Map = null;

            EntityUnregistered?.Invoke(this, new EntityEventArgs(entityComponent));
        }

        #region OdinInspector

        void OnValidate()
        {
            NavComponent = GetComponent<NavComponent>();
        }

        #endregion
    }
}