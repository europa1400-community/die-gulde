using System;
using GuldeLib.Timing;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace GuldeLib.Players
{
    public class ActionComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        [MinValue(1)]
        public int PointsPerRound { get; set; }

        [ShowInInspector]
        [BoxGroup("Info")]
        [MinValue(0)]
        public int Points { get; set; }

        public event EventHandler<ActionEventArgs> PointsChanged;

        void Awake()
        {
            this.Log("Action initialized");
        }

        void Start()
        {
            if (Locator.Time) Locator.Time.YearTicked += OnYearTicked;
        }

        void OnYearTicked(object sender, TimeEventArgs e)
        {
            this.Log($"Action adding {PointsPerRound} points");

            Points += PointsPerRound;
            PointsChanged?.Invoke(this, new ActionEventArgs(Points));
        }
    }
}