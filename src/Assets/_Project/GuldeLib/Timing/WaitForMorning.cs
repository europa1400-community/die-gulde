using System;
using UnityEngine;

namespace GuldeLib.Timing
{
    public class WaitForMorning : CustomYieldInstruction
    {
        TimeComponent Time { get; }
        bool IsMorning { get; set; }

        public override bool keepWaiting => !IsMorning;

        public WaitForMorning(TimeComponent time)
        {
            Time = time;
            Time.Morning += OnMorning;
        }

        void OnMorning(object sender, EventArgs e)
        {
            IsMorning = true;
        }
    }
}