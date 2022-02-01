using System;
using System.Collections.Generic;
using GuldeLib.Timing;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Society
{
    [Serializable]
    public class TalentComponent : SerializedMonoBehaviour
    {
        public const int MIN_TALENT_POINTS = 0;
        public const int MAX_TALENT_POINTS = 12;

        public enum TalentType
        {
            Negotiation,
            Handicraft,
            Stealth,
            Combat,
            Rhetoric,
        }

        public Dictionary<TalentType, int> TalentToPoints { get; } = new Dictionary<TalentType, int>
        {
            { TalentType.Negotiation, 0 },
            { TalentType.Handicraft, 0 },
            { TalentType.Stealth, 0 },
            { TalentType.Combat, 0 },
            { TalentType.Rhetoric, 0 },
        };

        List<DelayedTalentPointChange> DelayedTalentPointChanges { get; } = new List<DelayedTalentPointChange>();

        public int Negotiation
        {
            get => GetTalentPoints(TalentType.Negotiation);
            set => SetTalentPoints(TalentType.Negotiation, value);
        }

        public int Handicraft
        {
            get => GetTalentPoints(TalentType.Handicraft);
            set => SetTalentPoints(TalentType.Handicraft, value);
        }

        public int Stealth
        {
            get => GetTalentPoints(TalentType.Stealth);
            set => SetTalentPoints(TalentType.Stealth, value);
        }

        public int Combat
        {
            get => GetTalentPoints(TalentType.Combat);
            set => SetTalentPoints(TalentType.Combat, value);
        }

        public int Rhetoric
        {
            get => GetTalentPoints(TalentType.Rhetoric);
            set => SetTalentPoints(TalentType.Rhetoric, value);
        }

        public event EventHandler<InitializedEventArgs> Initialized;

        public event EventHandler<TalentPointsChangedEventArgs> TalentPointsChanged;

        void Start()
        {
            Locator.Time.YearTicked += OnYearTicked;
            Initialized?.Invoke(this, new InitializedEventArgs());
        }

        public int GetTalentPoints(TalentType talentType) =>
            TalentToPoints[talentType];

        public void SetTalentPoints(TalentType talentType, int points)
        {
            var currentPoints = GetTalentPoints(talentType);
            if (points == currentPoints) return;

            TalentToPoints[talentType] = Mathf.Clamp(points, MIN_TALENT_POINTS, MAX_TALENT_POINTS);
            TalentPointsChanged?.Invoke(this, new TalentPointsChangedEventArgs(talentType, points));
        }

        public void IncreaseTalentPoints(TalentType talentType, int amount)
        {
            if (amount == 0) return;

            var currentPoints = GetTalentPoints(talentType);
            SetTalentPoints(talentType, currentPoints + amount);
        }

        public void DecreaseTalentPoints(TalentType talentType, int amount) =>
            IncreaseTalentPoints(talentType, -amount);

        public void IncreaseTalentPointsForRounds(TalentType talentType, int amount, int rounds)
        {
            IncreaseTalentPoints(talentType, amount);

            var delayedTalentPointChange = new DelayedTalentPointChange(talentType, amount, rounds);
            DelayedTalentPointChanges.Add(delayedTalentPointChange);
        }

        public void DecreaseTalentPointsForRounds(TalentType talentType, int amount, int rounds) =>
            IncreaseTalentPointsForRounds(talentType, amount, -rounds);

        public void OnYearTicked(object sender, TimeComponent.TimeEventArgs e)
        {
            foreach (var delayedTalentPointChange in DelayedTalentPointChanges)
            {
                delayedTalentPointChange.Rounds -= 1;

                if (delayedTalentPointChange.Rounds > 0) continue;
                DecreaseTalentPoints(delayedTalentPointChange.TalentType, delayedTalentPointChange.Amount);
            }

            DelayedTalentPointChanges.RemoveAll(delayedTalentPointChange =>
                delayedTalentPointChange.Rounds <= 0);
        }

        public class DelayedTalentPointChange
        {
            public TalentComponent.TalentType TalentType { get; }
            public int Amount { get; }
            public int Rounds { get; set; }

            public DelayedTalentPointChange(TalentComponent.TalentType talentType, int amount, int rounds)
            {
                TalentType = talentType;
                Amount = amount;
                Rounds = rounds;
            }
        }

        public class InitializedEventArgs : EventArgs
        {

        }

        public class TalentPointsChangedEventArgs : EventArgs
        {
            public TalentType TalentType { get; }
            public int TalentPoints { get; }

            public TalentPointsChangedEventArgs(TalentType talentType, int talentPoints) =>
                (TalentType, TalentPoints) = (talentType, talentPoints);
        }
    }
}