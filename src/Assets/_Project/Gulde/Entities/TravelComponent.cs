using System;
using Gulde.Maps;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Entities
{
    [HideMonoScript]
    [RequireComponent(typeof(EntityComponent))]
    [RequireComponent(typeof(PathfindingComponent))]
    public class TravelComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public LocationComponent CurrentDestination { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Info")]
        public EntityComponent Entity { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Info")]
        public PathfindingComponent Pathfinding { get; set; }

        public event EventHandler<LocationEventArgs> LocationReached;

        public WaitForLocationReached WaitForLocationReached => new WaitForLocationReached(this);

        public void Awake()
        {
            Entity = GetComponent<EntityComponent>();
            Pathfinding = GetComponent<PathfindingComponent>();

            Pathfinding.DestinationReached += OnDestinationReached;
        }

        public void TravelTo(LocationComponent location)
        {
            Debug.Log($"Travelling {name} to {location.name}");

            if (Entity.Location)
            {
                Debug.Log($"Unregistering {name} in location {Entity.Location}");
                Entity.Location.EntityRegistry.Unregister(Entity);
            }

            CurrentDestination = location;

            Pathfinding.SetDestination(location.EntryCell);
        }

        void OnDestinationReached(object sender, CellEventArgs e)
        {
            Debug.Log($"{name} reached location {CurrentDestination}");

            if (!CurrentDestination) return;

            CurrentDestination.EntityRegistry.Register(Entity);

            LocationReached?.Invoke(this, new LocationEventArgs(CurrentDestination));
        }
    }

    public class WaitForLocationReached : CustomYieldInstruction
    {
        TravelComponent Travel { get; }
        bool IsLocationReached { get; set; }

        public override bool keepWaiting => !IsLocationReached && Travel.CurrentDestination != Travel.Entity.Location;

        public WaitForLocationReached(TravelComponent travel)
        {
            Travel = travel;
            Travel.LocationReached += OnLocationReached;
        }

        void OnLocationReached(object sender, LocationEventArgs e)
        {
            IsLocationReached = true;
        }
    }
}