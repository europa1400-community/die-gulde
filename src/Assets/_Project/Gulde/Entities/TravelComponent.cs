using System;
using Gulde.Logging;
using Gulde.Maps;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Entities
{
    [RequireComponent(typeof(EntityComponent))]
    [RequireComponent(typeof(PathfindingComponent))]
    public class TravelComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public LocationComponent CurrentDestination { get; private set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public EntityComponent Entity { get; private set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public PathfindingComponent Pathfinding { get; private set; }

        public event EventHandler<LocationEventArgs> LocationReached;

        public WaitForLocationReached WaitForLocationReached => new WaitForLocationReached(this);

        public void Awake()
        {
            this.Log("Travel initializing");

            Entity = GetComponent<EntityComponent>();
            Pathfinding = GetComponent<PathfindingComponent>();

            Pathfinding.DestinationReached += OnDestinationReached;
        }

        public void TravelTo(LocationComponent location)
        {
            this.Log(Entity.Location
                ? $"Travel travelling from {location} to {location}"
                : $"Travel spawning entity at {location}");

            if (Entity.Location)
            {
                Entity.Location.EntityRegistry.Unregister(Entity);
            }

            CurrentDestination = location;

            Pathfinding.SetDestination(location.EntryCell);
        }

        void OnDestinationReached(object sender, CellEventArgs e)
        {
            this.Log(CurrentDestination
                ? $"Travel reached location {CurrentDestination}"
                : $"Travel reached missing location");

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