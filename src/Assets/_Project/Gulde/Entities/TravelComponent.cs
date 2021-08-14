using Gulde.Maps;
using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using FilePathAttribute = UnityEditor.FilePathAttribute;

namespace Gulde.Entities
{
    [HideMonoScript]
    [RequireComponent(typeof(EntityComponent))]
    [RequireComponent(typeof(PathfindingComponent))]
    public class TravelComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ReadOnly]
        EntityComponent Entity { get; set; }

        [OdinSerialize]
        [ReadOnly]
        PathfindingComponent Pathfinding { get; set; }

        LocationComponent CurrentDestination { get; set; }

        void Awake()
        {
            Entity = GetComponent<EntityComponent>();
            Pathfinding = GetComponent<PathfindingComponent>();

            Pathfinding.DestinationReached -= OnDestinationReached;

            Pathfinding.DestinationReached += OnDestinationReached;
        }

        public void TravelTo(LocationComponent location)
        {
            if (Entity.Location) Entity.Location.EntityRegistry.Unregister(Entity);

            CurrentDestination = location;

            Pathfinding.SetDestination(location.EntryCell);
        }

        void OnDestinationReached(object sender, CellEventArgs e)
        {
            if (!CurrentDestination) return;

            CurrentDestination.EntityRegistry.Register(Entity);
        }

        #region OdinInspector

        void OnValidate()
        {
            Entity = GetComponent<EntityComponent>();
            Pathfinding = GetComponent<PathfindingComponent>();

            Pathfinding.DestinationReached -= OnDestinationReached;

            Pathfinding.DestinationReached += OnDestinationReached;
        }

        #endregion
    }
}