using System;
using System.Collections.Generic;
using Gulde.Company;
using Gulde.Logging;
using Gulde.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Entities
{
    public class EntityRegistryComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<EntityComponent> Entities { get; private set; } = new HashSet<EntityComponent>();

        public event EventHandler<EntityEventArgs> Registered;
        public event EventHandler<EntityEventArgs> Unregistered;

        public bool IsRegistered(EntityComponent entityComponent) => Entities.Contains(entityComponent);

        public void Register(EntityComponent entity)
        {
            if (!entity) return;

            this.Log($"Registry registering entity {entity}");

            Entities.Add(entity);

            Registered?.Invoke(this, new EntityEventArgs(entity));
        }

        public void Unregister(EntityComponent entity)
        {
            if (!entity) return;

            this.Log($"Registry unregistering entity {entity}");

            Entities.Remove(entity);

            Unregistered?.Invoke(this, new EntityEventArgs(entity));
        }
    }
}