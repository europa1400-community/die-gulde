using System;

namespace Gulde.Player
{
    public class ActionEventArgs : EventArgs
    {
        public ActionEventArgs(int points)
        {
            Points = points;
        }

        public int Points { get; }
    }
}