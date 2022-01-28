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
        public ActionComponent Action => GetComponent<ActionComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public WealthComponent Wealth => GetComponent<WealthComponent>();

        void Awake()
        {
            this.Log("Player initializing");
        }
    }
}