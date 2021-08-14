using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Entities
{
    [HideMonoScript]
    public class EntityRegistryComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ListDrawerSettings(Expanded = true)]
        public HashSet<EntityComponent> Entities { get; private set; } = new HashSet<EntityComponent>();

        public event EventHandler<EntityEventArgs> Registered;
        public event EventHandler<EntityEventArgs> Unregistered;

        public bool IsRegistered(EntityComponent entityComponent) => Entities.Contains(entityComponent);

        public void Register(EntityComponent entityComponent)
        {
            if (!entityComponent) return;

            Entities.Add(entityComponent);

            Registered?.Invoke(this, new EntityEventArgs(entityComponent));

            Debug.Log($"{name} registered the entity {entityComponent.name}");
        }

        public void Unregister(EntityComponent entityComponent)
        {
            if (!entityComponent) return;

            Entities.Remove(entityComponent);

            Unregistered?.Invoke(this, new EntityEventArgs(entityComponent));

            Debug.Log($"{name} unregistered the entity {entityComponent.name}");
        }
    }
}