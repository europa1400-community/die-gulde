using System;
using GuldeLib.Economy;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GuldeLib.Players
{
    public class CitizenComponent : SerializedMonoBehaviour
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

        public class InitializedEventArgs : EventArgs
        {
        }
    }
}