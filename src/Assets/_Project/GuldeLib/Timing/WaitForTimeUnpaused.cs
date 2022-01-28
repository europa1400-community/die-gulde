using UnityEngine;

namespace GuldeLib.Timing
{
    public class WaitForTimeUnpaused : CustomYieldInstruction
    {
        public override bool keepWaiting => Time.timeScale == 0f;
    }
}