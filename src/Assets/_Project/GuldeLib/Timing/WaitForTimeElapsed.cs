using System.Timers;
using UnityEngine;

namespace GuldeLib.Timing
{
    public class WaitForTimeElapsed : CustomYieldInstruction
    {
        public override bool keepWaiting => !TimerElapsed;
        bool TimerElapsed { get; set; }
        Timer Timer { get; }

        public WaitForTimeElapsed(float seconds)
        {
            Timer = new Timer(seconds * 1000);
            Timer.Elapsed += OnTimerElapsed;
            Timer.Start();
        }

        void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            TimerElapsed = true;
            Timer.Dispose();
        }
    }
}