using System;

namespace GuldeLib.Players
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