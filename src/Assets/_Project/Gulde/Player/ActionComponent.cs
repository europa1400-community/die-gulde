using System;
using Gulde.Timing;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gulde.Player
{
    public class ActionComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [MinValue(0)]
        public int Points { get; set; }

        [OdinSerialize]
        [MinValue(1)]
        public int PointsPerRound { get; set; }

        public event EventHandler<ActionEventArgs> PointsChanged;

        void Awake()
        {
            if (Locator.Time) Locator.Time.YearTicked += OnYearTicked;
        }

        void OnYearTicked(object sender, TimeEventArgs e)
        {
            Points += PointsPerRound;
            PointsChanged?.Invoke(this, new ActionEventArgs(Points));
        }
    }
}