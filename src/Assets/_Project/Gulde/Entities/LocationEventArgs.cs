using System;
using Gulde.Maps;

namespace Gulde.Entities
{
    public class LocationEventArgs : EventArgs
    {
        public LocationEventArgs(LocationComponent location)
        {
            Location = location;
        }

        public LocationComponent Location { get; }
    }
}