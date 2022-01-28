using UnityEngine;

namespace GuldeLib.Pathfinding
{
    public class WaitForDestinationReached : CustomYieldInstruction
    {
        public WaitForDestinationReached(PathfinderComponent pathfinding)
        {
            Pathfinding = pathfinding;
            Pathfinding.DestinationReached += OnDestinationReached;
        }

        void OnDestinationReached(object sender, CellEventArgs e)
        {
            IsDestinationReached = true;
        }

        PathfinderComponent Pathfinding { get; }
        bool IsDestinationReached { get; set; }
        public override bool keepWaiting =>
            !IsDestinationReached && Pathfinding.Waypoints.Count != 0;
    }
}