using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Economy;
using Gulde.Entities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gulde.Maps
{
    public class LocationComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ListDrawerSettings(Expanded = true)]
        HashSet<EntityComponent> Entities { get; set; } = new HashSet<EntityComponent>();

        [ShowInInspector]
        [ListDrawerSettings(Expanded = true)]
        public List<ExchangeComponent> Exchanges => GetComponentsInChildren<ExchangeComponent>().ToList();

        public event EventHandler<EntityEventArgs> EntityRegistered;
        public event EventHandler<EntityEventArgs> EntityUnregistered;

        public bool IsEntityRegistered(EntityComponent entityComponent) => Entities.Contains(entityComponent);

        public void RegisterEntity(EntityComponent entityComponent)
        {
            if (!entityComponent) return;

            Entities.Add(entityComponent);

            entityComponent.Location = this;

            EntityRegistered?.Invoke(this, new EntityEventArgs(entityComponent));
        }

        public void UnregisterEntity(EntityComponent entityComponent)
        {
            if (!entityComponent) return;

            Entities.Remove(entityComponent);

            entityComponent.Location = null;

            EntityUnregistered?.Invoke(this, new EntityEventArgs(entityComponent));
        }
    }
}