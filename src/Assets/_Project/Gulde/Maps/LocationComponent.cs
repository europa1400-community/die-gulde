using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Economy;
using Gulde.Entities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Maps
{
    [RequireComponent(typeof(EntityRegistryComponent))]
    public class LocationComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        public Vector3Int EntryCell { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        public GameObject MapPrefab { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [BoxGroup("Info")]
        public MapComponent ContainingMap { get; private set; }

        [OdinSerialize]
        [BoxGroup("Info")]
        public MapComponent AssociatedMap { get; private set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        public List<ExchangeComponent> Exchanges => GetComponentsInChildren<ExchangeComponent>().ToList();

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public EntityRegistryComponent EntityRegistry { get; private set; }

        HashSet<EntityComponent> Entities => EntityRegistry.Entities;

        void Awake()
        {
            EntityRegistry = GetComponent<EntityRegistryComponent>();
            ContainingMap = GetComponentInParent<MapComponent>();

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
            e.Entity.SetLocation(this);
            if (AssociatedMap) AssociatedMap.EntityRegistry.Register(e.Entity);
        }

        void OnEntityUnregistered(object sender, EntityEventArgs e)
        {
            e.Entity.SetLocation(null);
            if (AssociatedMap) AssociatedMap.EntityRegistry.Unregister(e.Entity);
        }
    }
}