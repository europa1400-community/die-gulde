using System;
using System.Collections.Generic;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;

namespace GuldeLib.Entities
{
    public class EntityRegistryComponent : SerializedMonoBehaviour
    {

        [ShowInInspector]
        [BoxGroup("Info")]
        public HashSet<EntityComponent> Entities { get; } = new HashSet<EntityComponent>();

        public event EventHandler<EntityEventArgs> Registered;
        public event EventHandler<EntityEventArgs> Unregistered;
        public event EventHandler<InitializedEventArgs> Initialized;

        public bool IsRegistered(EntityComponent entityComponent) => Entities.Contains(entityComponent);

        void Start()
        {
            Initialized?.Invoke(this, new InitializedEventArgs());
        }

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

        public class InitializedEventArgs : EventArgs
        {
        }

        public class EntityEventArgs : EventArgs
        {
            public EntityComponent Entity { get; }

            public EntityEventArgs(EntityComponent entity) => Entity = entity;
        }
    }
}