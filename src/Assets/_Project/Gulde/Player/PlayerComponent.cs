using Gulde.Economy;
using Gulde.Logging;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Player
{
    [RequireComponent(typeof(ActionComponent))]
    [RequireComponent(typeof(WealthComponent))]
    public class PlayerComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public ActionComponent Action { get; private set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public WealthComponent Wealth { get; private set; }

        void Awake()
        {
            this.Log("Player initializing");

            Action = GetComponent<ActionComponent>();
            Wealth = GetComponent<WealthComponent>();

            Locator.Player = this;
        }
    }
}