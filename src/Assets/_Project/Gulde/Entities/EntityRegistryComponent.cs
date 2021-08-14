using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

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
        }

        public void Unregister(EntityComponent entityComponent)
        {
            if (!entityComponent) return;

            Entities.Remove(entityComponent);

            Unregistered?.Invoke(this, new EntityEventArgs(entityComponent));
        }
    }
}