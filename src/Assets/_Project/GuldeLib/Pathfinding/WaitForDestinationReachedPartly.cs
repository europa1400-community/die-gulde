using UnityEngine;

namespace GuldeLib.Pathfinding
{
    public class WaitForDestinationReachedPartly : CustomYieldInstruction
    {
        public WaitForDestinationReachedPartly(PathfinderComponent pathfinding, float percentage)
        {
            Pathfinding = pathfinding;
            Percentage = percentage;
        }
        PathfinderComponent Pathfinding { get; }
        float Percentage { get; }
        public override bool keepWaiting => Pathfinding.TravelPercentage < Percentage;
    }
}