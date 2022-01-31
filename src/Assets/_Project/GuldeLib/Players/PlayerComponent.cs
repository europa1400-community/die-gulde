using System;
using GuldeLib.Economy;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Players
{
    [RequireComponent(typeof(ActionPointComponent))]
    [RequireComponent(typeof(WealthComponent))]
    public class PlayerComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public ActionPointComponent ActionPoint => GetComponent<ActionPointComponent>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public WealthComponent Wealth => GetComponent<WealthComponent>();

        public event EventHandler<InitializedEventArgs> Initialized;

        void Start()
        {
            Initialized?.Invoke(this, new InitializedEventArgs());
        }

        public class InitializedEventArgs
        {
        }
    }
}