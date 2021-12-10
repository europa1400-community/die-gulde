using GuldeLib.Maps;
using UnityEngine;

namespace GuldeLib.Entities
{
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