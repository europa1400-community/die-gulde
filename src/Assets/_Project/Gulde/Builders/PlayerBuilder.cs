using System.Collections;
using Gulde.Inventory;
using UnityEditor;
using UnityEngine;

namespace Gulde.Builders
{
    public class PlayerBuilder : Builder
    {
        public GameObject PlayerObject { get; private set; }

        [LoadAsset("prefab_player")]
        GameObject PlayerPrefab { get; set; }

        int Slots { get; set; }

        public PlayerBuilder() : base()
        {
        }

        public PlayerBuilder WithSlots(int slots)
        {
            Slots = slots;
            return this;
        }

        public override IEnumerator Build()
        {
            yield return base.Build();

            PlayerObject = Object.Instantiate(PlayerPrefab);

            var inventory = PlayerObject.GetComponent<InventoryComponent>();
            inventory.Slots = Slots;
        }
    }
}