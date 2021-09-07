using System;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Economy;
using GuldeLib.Entities;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Maps
{
    [RequireComponent(typeof(EntityRegistryComponent))]
    public class LocationComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        public Vector3Int EntryCell { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        public GameObject MapPrefab { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public MapComponent ContainingMap { get; private set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public MapComponent AssociatedMap { get; private set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public List<ExchangeComponent> Exchanges => GetComponentsInChildren<ExchangeComponent>().ToList();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public EntityRegistryComponent EntityRegistry { get; private set; }

        HashSet<EntityComponent> Entities => EntityRegistry.Entities;

        public event EventHandler<EntityEventArgs> EntitySpawned;
        public event EventHandler<EntityEventArgs> EntityArrived;
        public event EventHandler<EntityEventArgs> EntityLeft;
        public event EventHandler<MapEventArgs> ContainingMapChanged;

        void Awake()
        {
            this.Log("Location initializing");

            EntityRegistry = GetComponent<EntityRegistryComponent>();

            if (MapPrefab)
            {
                var mapObject = Instantiate(MapPrefab, ContainingMap.transform.parent);
                AssociatedMap = mapObject.GetComponent<MapComponent>();
            }

            EntityRegistry.Registered += OnEntityRegistered;
            EntityRegistry.Unregistered += OnEntityUnregistered;
        }

        void OnEntityRegistered(object sender, EntityEventArgs e)
        {
            this.Log(e.Entity.Map
            ? $"Location registering {e.Entity}"
            : $"Location spawning {e.Entity}");

            e.Entity.SetLocation(this);

            if (!e.Entity.Map) e.Entity.SetCell(EntryCell);

            if (AssociatedMap) AssociatedMap.EntityRegistry.Register(e.Entity);

            if (!e.Entity.Map)
            {
                EntitySpawned?.Invoke(this, new EntityEventArgs(e.Entity));
                return;
            }

            EntityArrived?.Invoke(this, new EntityEventArgs(e.Entity));
        }

        void OnEntityUnregistered(object sender, EntityEventArgs e)
        {
            this.Log($"Location unregistering {e.Entity}");

            e.Entity.SetLocation(null);

            if (AssociatedMap)
            {
                this.Log($"Location relocating {e.Entity} from {AssociatedMap} to {ContainingMap}");

                AssociatedMap.EntityRegistry.Unregister(e.Entity);
                ContainingMap.EntityRegistry.Register(e.Entity);
            }

            EntityLeft?.Invoke(this, new EntityEventArgs(e.Entity));
        }

        public void SetContainingMap(MapComponent map)
        {
            this.Log($"Location setting containing map to {map}");

            ContainingMap = map;

            ContainingMapChanged?.Invoke(this, new MapEventArgs(map));
        }
    }
}