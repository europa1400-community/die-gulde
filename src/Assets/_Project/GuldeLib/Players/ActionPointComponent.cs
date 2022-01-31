using System;
using System.Collections.Generic;
using GuldeLib.Timing;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Players
{
    public class ActionPointComponent : SerializedMonoBehaviour
    {
        int pointsPerRound;
        [ShowInInspector]
        [BoxGroup("Settings")]
        [MinValue(1)]
        public int PointsPerRound
        {
            get => pointsPerRound;
            set
            {
                pointsPerRound = Mathf.Max(0, pointsPerRound + value);
                PointsPerRoundChanged?.Invoke(this, new ActionPointsPerRoundChangedEventArgs(value));
            }
        }

        int points;
        [ShowInInspector]
        [BoxGroup("Info")]
        [MinValue(0)]
        public int Points
        {
            get => points;
            set
            {
                points = Mathf.Max(0, points + value);
                PointsChanged?.Invoke(this, new ActionPointsChangedEventArgs(points));
            }
        }

        List<DelayedPointsPerRoundChange> DelayedPointsPerRoundChanges { get; } = new List<DelayedPointsPerRoundChange>();

        public event EventHandler<InitializedEventArgs> Initialized;
        public event EventHandler<ActionPointsChangedEventArgs> PointsChanged;
        public event EventHandler<ActionPointsPerRoundChangedEventArgs> PointsPerRoundChanged;


        void Start()
        {
            Initialized?.Invoke(this, new InitializedEventArgs(Points, PointsPerRound));
        }

        /// <summary>
        /// Shorthand for writing Points += amount.
        /// </summary>
        /// <param name="amount">The amount of points to increase by.</param>
        public void IncreasePoints(int amount) =>
            Points += amount;

        public void DecreasePoints(int amount) =>
            IncreasePoints(-amount);

        public void IncreasePointsPerRound(int amount) =>
            PointsPerRound += amount;

        public void DecreasePointsPerRound(int amount) =>
            IncreasePointsPerRound(-amount);

        public void IncreasePointsPerRoundForRounds(int rounds, int amount)
        {
            IncreasePointsPerRound(amount);

            var delayedPointsPerRoundChange = new DelayedPointsPerRoundChange(rounds, amount);
            DelayedPointsPerRoundChanges.Add(delayedPointsPerRoundChange);
        }

        public void DecreasePointsPerRoundForRounds(int rounds, int amount) =>
            IncreasePointsPerRoundForRounds(rounds, -amount);

        public void OnYearTicked(object sender, TimeComponent.TimeEventArgs e)
        {
            this.Log($"Adding {PointsPerRound} points");

            IncreasePoints(PointsPerRound);

            foreach (var delayedPointsPerRoundChange in DelayedPointsPerRoundChanges)
            {
                delayedPointsPerRoundChange.Rounds -= 1;

                if (delayedPointsPerRoundChange.Rounds > 0) continue;
                DecreasePointsPerRound(delayedPointsPerRoundChange.Amount);
            }

            DelayedPointsPerRoundChanges.RemoveAll(delayedPointsPerRoundChange => delayedPointsPerRoundChange.Rounds <= 0);
        }

        [Serializable]
        public class DelayedPointsPerRoundChange
        {
            [ShowInInspector]
            public int Rounds { get; set; }

            [ShowInInspector]
            public int Amount { get; set; }

            public DelayedPointsPerRoundChange(int rounds, int amount)
            {
                Rounds = rounds;
                Amount = amount;
            }
        }

        public class InitializedEventArgs
        {
            public int Points { get; }

            public int PointsPerRound { get; }

            public InitializedEventArgs(int points, int pointsPerRound)
            {
                Points = points;
                PointsPerRound = pointsPerRound;
            }
        }

        public class ActionPointsChangedEventArgs : EventArgs
        {
            public ActionPointsChangedEventArgs(int points)
            {
                Points = points;
            }

            public int Points { get; }
        }

        public class ActionPointsPerRoundChangedEventArgs : EventArgs
        {
            public int Amount { get; }

            public ActionPointsPerRoundChangedEventArgs(int amount) =>
                Amount = amount;
        }
    }
}