using Gulde.Economy;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Player
{
    [RequireComponent(typeof(ActionComponent))]
    [RequireComponent(typeof(WealthComponent))]
    public class PlayerComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ReadOnly]
        public ActionComponent Action { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        public WealthComponent Wealth { get; private set; }

        void Awake()
        {
            Action = GetComponent<ActionComponent>();
            Wealth = GetComponent<WealthComponent>();

            Locator.Player = this;
        }
    }
}