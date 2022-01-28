using UnityEngine;

namespace GuldeLib.Timing
{
    public class WaitForMinuteTicked : CustomYieldInstruction
    {
        TimeComponent Time { get; }
        bool MinuteTicked { get; set; }

        public override bool keepWaiting => !MinuteTicked;

        public WaitForMinuteTicked(TimeComponent time)
        {
            Time = time;
            Time.MinuteTicked += OnMinuteTicked;
        }

        void OnMinuteTicked(object sender, TimeEventArgs e)
        {
            MinuteTicked = true;
        }
    }
}