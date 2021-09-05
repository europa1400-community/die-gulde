using System;

namespace GuldeLib.Timing
{
    public class TimeEventArgs : EventArgs
    {
        public TimeEventArgs(int minute, int hour, int year)
        {
            Minute = minute;
            Hour = hour;
            Year = year;
        }

        public int Minute { get; }
        public int Hour { get; }
        public int Year { get; }
        
    }
}