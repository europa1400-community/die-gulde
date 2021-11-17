using GuldeLib.Economy;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Players
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