using System;

namespace Gulde.Maps
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