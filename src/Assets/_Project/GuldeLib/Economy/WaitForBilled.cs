using UnityEngine;

namespace GuldeLib.Economy
{
    /// <summary>
    /// <see cref = "CustomYieldInstruction"/> that waits for
    /// the <see cref = "WealthComponent.Billed"/> event to be invoked.
    /// </summary>
    public class WaitForBilled : CustomYieldInstruction
    {
        /// <summary>
        /// Gets or sets whether the <see cref = "WealthComponent.Billed"/> event has been invoked yet.
        /// </summary>
        bool IsBilled { get; set; }

        /// <inheritdoc cref="keepWaiting"/>
        public override bool keepWaiting => !IsBilled;

        /// <summary>
        /// Initializes a new instance of the <see cref = "WaitForBilled"/> class.
        /// Sets up event callbacks.
        /// </summary>
        /// <param name="wealthComponent"></param>
        public WaitForBilled(WealthComponent wealthComponent)
        {
            wealthComponent.Billed += OnBilled;
        }

        /// <summary>
        /// Callback for the <see cref = "WealthComponent.Billed"/> event.
        /// </summary>
        /// <remarks>
        /// When this occurs, the <see cref = "CustomYieldInstruction"/> stops waiting.
        /// </remarks>
        void OnBilled(object sender, BillingEventArgs e)
        {
            IsBilled = true;
        }
    }
}