using System;
using GuldeLib.Maps;
using GuldeLib.Pathfinding;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Entities
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

        public event EventHandler<LocationEventArgs> DestinationChanged;
        public event EventHandler<LocationEventArgs> DestinationReached;

        public WaitForDestinationReached WaitForDestinationReached => new WaitForDestinationReached(this);

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
                ? $"Travel travelling from {Entity.Location} to {location}"
                : $"Travel spawning entity at {location}");

            if (!location) return;

            if (Entity.Location)
            {
                Entity.Location.EntityRegistry.Unregister(Entity);
            }

            CurrentDestination = location;

            Pathfinding.SetDestination(location.EntryCell);
            DestinationChanged?.Invoke(this, new LocationEventArgs(location));
        }

        void OnDestinationReached(object sender, CellEventArgs e)
        {
            this.Log(CurrentDestination
                ? $"Travel reached location {CurrentDestination}"
                : $"Travel reached missing location");

            if (!CurrentDestination) return;

            CurrentDestination.EntityRegistry.Register(Entity);

            DestinationReached?.Invoke(this, new LocationEventArgs(CurrentDestination));
        }
    }

    public class WaitForDestinationReached : CustomYieldInstruction
    {
        TravelComponent Travel { get; }
        bool IsDestinationReached { get; set; }

        public override bool keepWaiting => !IsDestinationReached && Travel.CurrentDestination != Travel.Entity.Location;

        public WaitForDestinationReached(TravelComponent travel)
        {
            Travel = travel;
            Travel.DestinationReached += OnDestinationReached;
        }

        void OnDestinationReached(object sender, LocationEventArgs e)
        {
            IsDestinationReached = true;
        }
    }
}