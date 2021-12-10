using System;
using GuldeLib.Maps;
using GuldeLib.Pathfinding;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;

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

        public event EventHandler<LocationEventArgs> DestinationChanged;
        public event EventHandler<LocationEventArgs> DestinationReached;

        public WaitForDestinationReached WaitForDestinationReached => new WaitForDestinationReached(this);

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
            DestinationChanged?.Invoke(this, new LocationEventArgs(location));
        }

        public void OnDestinationReached(object sender, CellEventArgs e)
        {
            this.Log(CurrentDestination
                ? $"Travel reached location {CurrentDestination}"
                : $"Travel reached missing location");

            if (!CurrentDestination) return;

            CurrentDestination.EntityRegistry.Register(Entity);

            DestinationReached?.Invoke(this, new LocationEventArgs(CurrentDestination));
        }
    }
}