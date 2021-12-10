using System;
using UnityEngine;

namespace GuldeLib.Timing
{
    public class WaitForEvening : CustomYieldInstruction
    {
        TimeComponent Time { get; }
        bool IsEvening { get; set; }

        public override bool keepWaiting => !IsEvening;

        public WaitForEvening(TimeComponent time)
        {
            Time = time;
            Time.Evening += OnEvening;
        }

        void OnEvening(object sender, EventArgs e)
        {
            IsEvening = true;
        }
    }
}