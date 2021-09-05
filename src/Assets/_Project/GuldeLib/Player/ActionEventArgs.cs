using System;

namespace GuldeLib.Player
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