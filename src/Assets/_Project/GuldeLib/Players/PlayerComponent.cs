using GuldeLib.Economy;
using MonoExtensions.Runtime;
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
        public ActionComponent Action => this.GetCachedComponent<ActionComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public WealthComponent Wealth => this.GetCachedComponent<WealthComponent>();

        void Awake()
        {
            this.Log("Player initializing");
            Locator.Player = this;
        }
    }
}