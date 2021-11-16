using System.Collections;
using GuldeLib.Inventories;
using UnityEngine;

namespace GuldeLib.Builders
{
    /// <summary>
    /// Provides functionality to build a player.
    /// </summary>
    public class PlayerBuilder : Builder
    {
        /// <summary>
        /// Gets the built player's GameObject.
        /// </summary>
        public GameObject PlayerObject { get; private set; }

        /// <summary>
        /// Gets or sets the prefab used to create the player.
        /// </summary>
        [LoadAsset("prefab_player")]
        GameObject PlayerPrefab { get; set; }

        /// <summary>
        /// Gets or sets the amount of slots in the player's <see cref = "InventoryComponent">inventory</see>.
        /// </summary>
        int Slots { get; set; }

        /// <summary>
        /// Sets the player's inventory slots to the given value.
        /// </summary>
        public PlayerBuilder WithSlots(int slots)
        {
            Slots = slots;
            return this;
        }

        /// <inheritdoc cref="Builder.Build"/>
        public override IEnumerator Build()
        {
            yield return base.Build();

            PlayerObject = Object.Instantiate(PlayerPrefab);

            var inventory = PlayerObject.GetComponent<InventoryComponent>();
            inventory.Slots = Slots;
        }
    }
}