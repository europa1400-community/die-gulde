using System;
using System.Collections.Generic;
using GuldeLib.Timing;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Society
{
    public class FavorComponent : SerializedMonoBehaviour
    {
        public const float MIN_FAVOR = 0f;
        public const float MAX_FAVOR = 1f;
        public const float DEFAULT_FAVOR = 0.5f;

        List<DelayedFavorChange> DelayedFavorChanges { get; } =
            new List<DelayedFavorChange>();

        Dictionary<FavorComponent, float> SocialComponentToFavor { get; } =
            new Dictionary<FavorComponent, float>();

        public event EventHandler<FavorChangedEventArgs> FavorChanged;

        public event EventHandler<InitializedEventArgs> Initialized;

        void Start()
        {
            Locator.Time.YearTicked += OnYearTicked;

            Initialized?.Invoke(this, new InitializedEventArgs(SocialComponentToFavor));
        }

        /// <summary>
        /// Whether a favor value exists for the specified <see cref = "FavorComponent"/>.
        /// </summary>
        bool HasFavor(FavorComponent favorComponent) =>
            SocialComponentToFavor.ContainsKey(favorComponent);

        /// <summary>
        /// Sets the <see cref = "DEFAULT_FAVOR"/> value for the specified <see cref = "FavorComponent"/>.
        /// </summary>
        void Register(FavorComponent favorComponent)
        {
            if (HasFavor(favorComponent)) return;
            SocialComponentToFavor.Add(favorComponent, DEFAULT_FAVOR);
        }

        /// <summary>
        /// Sets the specified favor value for the specified <see cref = "FavorComponent"/>
        /// </summary>
        /// <remarks>
        /// Registers the <see cref = "FavorComponent"/>.
        /// Clamps the favor value between <see cref = "MIN_FAVOR"/> and <see cref = "MAX_FAVOR"/>.
        /// Invokes the <see cref = "FavorChanged"/> event if the value changed.
        /// </remarks>
        public void SetFavor(FavorComponent favorComponent, float value)
        {
            Register(favorComponent);

            var currentFavor = GetFavor(favorComponent);
            if (Math.Abs(currentFavor - value) < float.Epsilon)
            {
                RemoveIfDefault(favorComponent);
                return;
            }

            SocialComponentToFavor[favorComponent] = Mathf.Clamp(value, MIN_FAVOR, MAX_FAVOR);
            RemoveIfDefault(favorComponent);

            FavorChanged?.Invoke(this, new FavorChangedEventArgs(favorComponent, value));
        }

        /// <summary>
        /// Returns the favor value for the specified <see cref = "FavorComponent"/>.
        /// </summary>
        /// <remarks>
        /// If the SocialComponent is not registered, the <see cref = "DEFAULT_FAVOR"/> value will be returned.
        /// </remarks>
        public float GetFavor(FavorComponent favorComponent) =>
            !HasFavor(favorComponent) ? DEFAULT_FAVOR : SocialComponentToFavor[favorComponent];

        /// <summary>
        /// Resets the favor value of the specified <see cref = "FavorComponent"/> to the <see cref = "DEFAULT_FAVOR"/> value.
        /// </summary>
        public void ResetFavor(FavorComponent favorComponent) =>
            SetFavor(favorComponent, DEFAULT_FAVOR);

        /// <summary>
        /// Removes the specified <see cref = "FavorComponent"/> from the <see cref = "SocialComponentToFavor"/> dictionary
        /// if the favor value equals the <see cref = "DEFAULT_FAVOR"/> value.
        /// </summary>
        void RemoveIfDefault(FavorComponent favorComponent)
        {
            if (!(Math.Abs(SocialComponentToFavor[favorComponent] - DEFAULT_FAVOR) < float.Epsilon)) return;
            SocialComponentToFavor.Remove(favorComponent);
        }

        /// <summary>
        /// Increases the favor value for the specified <see cref = "FavorComponent"/> by the specified amount.
        /// </summary>
        public void IncreaseFavor(FavorComponent favorComponent, float amount)
        {
            if (amount == 0) return;

            var currentFavor = GetFavor(favorComponent);
            SetFavor(favorComponent, currentFavor + amount);
        }

        /// <summary>
        /// Decreases the favor value for the specified <see cref = "FavorComponent"/> by the specified amount.
        /// </summary>
        public void DecreaseFavor(FavorComponent favorComponent, float amount) =>
            IncreaseFavor(favorComponent, -amount);

        /// <summary>
        /// Increases the favor value for the specified <see cref = "FavorComponent"/> by the specified amount for the specified number of rounds.
        /// </summary>
        public void IncreaseFavorForRounds(FavorComponent favorComponent, float amount, int rounds)
        {
            if (rounds < 1)
            {
                this.Log($"Favor for {favorComponent} cannot be increased for {rounds} rounds: rounds are less than 1", LogType.Warning);
                return;
            }

            IncreaseFavor(favorComponent, amount);

            var delayedFavorChange = new DelayedFavorChange(rounds, favorComponent, amount);
            DelayedFavorChanges.Add(delayedFavorChange);
        }

        /// <summary>
        /// Decreases the favor value for the specified <see cref = "FavorComponent"/> by the specified amount for the specified number of rounds.
        /// </summary>
        public void DecreaseFavorForRounds(FavorComponent favorComponent, float amount, int rounds) =>
            IncreaseFavorForRounds(favorComponent, -amount, rounds);

        void OnYearTicked(object sender, TimeComponent.TimeEventArgs e)
        {
            foreach (var delayedFavorChange in DelayedFavorChanges)
            {
                delayedFavorChange.Rounds -= 1;

                if (delayedFavorChange.Rounds > 0) continue;
                DecreaseFavor(delayedFavorChange.FavorComponent, delayedFavorChange.Amount);
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
            public FavorComponent FavorComponent { get; set; }

            [ShowInInspector]
            public float Amount { get; set; }

            public DelayedFavorChange(int rounds, FavorComponent favorComponent, float amount)
            {
                Rounds = rounds;
                FavorComponent = favorComponent;
                Amount = amount;
            }
        }

        public class InitializedEventArgs : EventArgs
        {
            public Dictionary<FavorComponent, float> SocialComponentToFavor { get; }

            public InitializedEventArgs(Dictionary<FavorComponent, float> socialComponentToFavor) =>
                SocialComponentToFavor = socialComponentToFavor;
        }

        public class FavorChangedEventArgs : EventArgs
        {
            public FavorComponent FavorComponent { get; }

            public float Value { get; }

            public FavorChangedEventArgs(FavorComponent favorComponent, float value)
            {
                FavorComponent = favorComponent;
                Value = value;
            }
        }
    }
}