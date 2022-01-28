using UnityEngine;

namespace GuldeLib.Timing
{
    public class WaitForWorkingHourTicked : CustomYieldInstruction
    {
        TimeComponent Time { get; }
        bool WorkingHourTicked { get; set; }

        public override bool keepWaiting => !WorkingHourTicked;

        public WaitForWorkingHourTicked(TimeComponent time)
        {
            Time = time;
            Time.WorkingHourTicked += OnWorkingHourTicked;
        }

        void OnWorkingHourTicked(object sender, TimeEventArgs e)
        {
            WorkingHourTicked = true;
        }
    }
}