using System;
using GuldeLib.Maps;
using GuldeLib.Pathfinding;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Entities
{
    public class TravelComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public LocationComponent CurrentDestination { get; private set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public EntityComponent Entity => GetComponent<EntityComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public PathfinderComponent Pathfinder => GetComponent<PathfinderComponent>();

        public event EventHandler<MapComponent.LocationEventArgs> DestinationChanged;
        public event EventHandler<MapComponent.LocationEventArgs> DestinationReached;

        void Awake()
        {
            this.Log("Travel initializing");
        }

        public void TravelTo(LocationComponent location)
        {
            if (!location) return;

            this.Log(Entity.Location
                ? $"Travel travelling from {Entity.Location} to {location}"
                : $"Travel spawning entity at {location}");

            if (Entity.Location)
            {
                Entity.Location.EntityRegistry.Unregister(Entity);
            }

            CurrentDestination = location;

            Pathfinder.SetDestination(location.EntryCell);
            DestinationChanged?.Invoke(this, new MapComponent.LocationEventArgs(location));
        }

        public void OnDestinationReached(object sender, PathfinderComponent.CellEventArgs e)
        {
            this.Log(CurrentDestination
                ? $"Travel reached location {CurrentDestination}"
                : $"Travel reached missing location");

            if (!CurrentDestination) return;

            CurrentDestination.EntityRegistry.Register(Entity);

            DestinationReached?.Invoke(this, new MapComponent.LocationEventArgs(CurrentDestination));
        }

        public class WaitForDestinationReached : CustomYieldInstruction
        {
            TravelComponent Travel { get; }
            bool IsDestinationReached { get; set; }

            public override bool keepWaiting =>
                !IsDestinationReached && Travel.CurrentDestination != Travel.Entity.Location;

            public WaitForDestinationReached(TravelComponent travel)
            {
                Travel = travel;
                Travel.DestinationReached += OnDestinationReached;
            }

            void OnDestinationReached(object sender, MapComponent.LocationEventArgs e)
            {
                IsDestinationReached = true;
            }
        }
    }
}