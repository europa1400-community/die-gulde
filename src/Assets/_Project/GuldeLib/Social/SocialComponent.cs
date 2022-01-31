using System;
using System.Collections.Generic;
using GuldeLib.Timing;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Social
{
    public class SocialComponent : SerializedMonoBehaviour
    {
        public const float MIN_FAVOR = 0f;
        public const float MAX_FAVOR = 1f;
        public const float DEFAULT_FAVOR = 0.5f;

        List<DelayedFavorChange> DelayedFavorChanges { get; } =
            new List<DelayedFavorChange>();

        Dictionary<SocialComponent, float> SocialComponentToFavor { get; } =
            new Dictionary<SocialComponent, float>();

        public event EventHandler<FavorChangedEventArgs> FavorChanged;

        public event EventHandler<InitializedEventArgs> Initialized;

        void Start()
        {
            Locator.Time.YearTicked += OnYearTicked;

            Initialized?.Invoke(this, new InitializedEventArgs(SocialComponentToFavor));
        }

        /// <summary>
        /// Whether a favor value exists for the specified <see cref = "SocialComponent"/>.
        /// </summary>
        bool HasFavor(SocialComponent socialComponent) =>
            SocialComponentToFavor.ContainsKey(socialComponent);

        /// <summary>
        /// Sets the <see cref = "DEFAULT_FAVOR"/> value for the specified <see cref = "SocialComponent"/>.
        /// </summary>
        void Register(SocialComponent socialComponent)
        {
            if (HasFavor(socialComponent)) return;
            SocialComponentToFavor.Add(socialComponent, DEFAULT_FAVOR);
        }

        /// <summary>
        /// Sets the specified favor value for the specified <see cref = "SocialComponent"/>
        /// </summary>
        /// <remarks>
        /// Registers the <see cref = "SocialComponent"/>.
        /// Clamps the favor value between <see cref = "MIN_FAVOR"/> and <see cref = "MAX_FAVOR"/>.
        /// Invokes the <see cref = "FavorChanged"/> event if the value changed.
        /// </remarks>
        public void SetFavor(SocialComponent socialComponent, float value)
        {
            Register(socialComponent);

            var currentFavor = GetFavor(socialComponent);
            if (Math.Abs(currentFavor - value) < float.Epsilon)
            {
                RemoveIfDefault(socialComponent);
                return;
            }

            SocialComponentToFavor[socialComponent] = Mathf.Clamp(value, MIN_FAVOR, MAX_FAVOR);
            RemoveIfDefault(socialComponent);

            FavorChanged?.Invoke(this, new FavorChangedEventArgs(socialComponent, value));
        }

        /// <summary>
        /// Returns the favor value for the specified <see cref = "SocialComponent"/>.
        /// </summary>
        /// <remarks>
        /// If the SocialComponent is not registered, the <see cref = "DEFAULT_FAVOR"/> value will be returned.
        /// </remarks>
        public float GetFavor(SocialComponent socialComponent) =>
            !HasFavor(socialComponent) ? DEFAULT_FAVOR : SocialComponentToFavor[socialComponent];

        /// <summary>
        /// Resets the favor value of the specified <see cref = "SocialComponent"/> to the <see cref = "DEFAULT_FAVOR"/> value.
        /// </summary>
        public void ResetFavor(SocialComponent socialComponent) =>
            SetFavor(socialComponent, DEFAULT_FAVOR);

        /// <summary>
        /// Removes the specified <see cref = "SocialComponent"/> from the <see cref = "SocialComponentToFavor"/> dictionary
        /// if the favor value equals the <see cref = "DEFAULT_FAVOR"/> value.
        /// </summary>
        void RemoveIfDefault(SocialComponent socialComponent)
        {
            if (!(Math.Abs(SocialComponentToFavor[socialComponent] - DEFAULT_FAVOR) < float.Epsilon)) return;
            SocialComponentToFavor.Remove(socialComponent);
        }

        /// <summary>
        /// Increases the favor value for the specified <see cref = "SocialComponent"/> by the specified amount.
        /// </summary>
        public void IncreaseFavor(SocialComponent socialComponent, float amount)
        {
            if (amount == 0) return;

            var currentFavor = GetFavor(socialComponent);
            SetFavor(socialComponent, currentFavor + amount);
        }

        /// <summary>
        /// Decreases the favor value for the specified <see cref = "SocialComponent"/> by the specified amount.
        /// </summary>
        public void DecreaseFavor(SocialComponent socialComponent, float amount) =>
            IncreaseFavor(socialComponent, -amount);

        /// <summary>
        /// Increases the favor value for the specified <see cref = "SocialComponent"/> by the specified amount for the specified number of rounds.
        /// </summary>
        public void IncreaseFavorForRounds(SocialComponent socialComponent, float amount, int rounds)
        {
            if (rounds < 1)
            {
                this.Log($"Favor for {socialComponent} cannot be increased for {rounds} rounds: rounds are less than 1", LogType.Warning);
                return;
            }

            IncreaseFavor(socialComponent, amount);

            var delayedFavorChange = new DelayedFavorChange(rounds, socialComponent, amount);
            DelayedFavorChanges.Add(delayedFavorChange);
        }

        /// <summary>
        /// Decreases the favor value for the specified <see cref = "SocialComponent"/> by the specified amount for the specified number of rounds.
        /// </summary>
        public void DecreaseFavorForRounds(SocialComponent socialComponent, float amount, int rounds) =>
            IncreaseFavorForRounds(socialComponent, -amount, rounds);

        void OnYearTicked(object sender, TimeComponent.TimeEventArgs e)
        {
            foreach (var delayedFavorChange in DelayedFavorChanges)
            {
                delayedFavorChange.Rounds -= 1;

                if (delayedFavorChange.Rounds > 0) continue;
                DecreaseFavor(delayedFavorChange.SocialComponent, delayedFavorChange.Amount);
            }

            DelayedFavorChanges.RemoveAll(delayedFavorChange =>
                delayedFavorChange.Rounds <= 0);
        }

        [Serializable]
        public class DelayedFavorChange
        {
            [ShowInInspector]
            public int Rounds { get; set; }

            [ShowInInspector]
            public SocialComponent SocialComponent { get; set; }

            [ShowInInspector]
            public float Amount { get; set; }

            public DelayedFavorChange(int rounds, SocialComponent socialComponent, float amount)
            {
                Rounds = rounds;
                SocialComponent = socialComponent;
                Amount = amount;
            }
        }

        public class InitializedEventArgs : EventArgs
        {
            public Dictionary<SocialComponent, float> SocialComponentToFavor { get; }

            public InitializedEventArgs(Dictionary<SocialComponent, float> socialComponentToFavor) =>
                SocialComponentToFavor = socialComponentToFavor;
        }

        public class FavorChangedEventArgs : EventArgs
        {
            public SocialComponent SocialComponent { get; }

            public float Value { get; }

            public FavorChangedEventArgs(SocialComponent socialComponent, float value)
            {
                SocialComponent = socialComponent;
                Value = value;
            }
        }
    }
}