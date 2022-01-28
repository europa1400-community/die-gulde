using UnityEngine;

namespace GuldeLib.Timing
{
    public class WaitForYearTicked : CustomYieldInstruction
    {
        TimeComponent Time { get; }
        bool HasYearTicked { get; set; }

        public override bool keepWaiting => !HasYearTicked;

        public WaitForYearTicked(TimeComponent time)
        {
            Time = time;
            Time.YearTicked += OnYearTicked;
        }

        void OnYearTicked(object sender, TimeEventArgs e)
        {
            HasYearTicked = true;
        }
    }
}