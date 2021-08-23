using Gulde.Company;
using Gulde.Economy;
using Gulde.Inventory;
using UnityEditor;
using UnityEngine;

namespace GuldePlayTests.Builders
{
    public class PlayerBuilder
    {
        GameObject PlayerObject { get; set; }

        public PlayerBuilder()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/prefab_player.prefab");
            PlayerObject = Object.Instantiate(prefab);
        }

        public PlayerBuilder WithSlots(int slots)
        {
            var inventoryComponent = PlayerObject.GetComponent<InventoryComponent>();
            inventoryComponent.Slots = slots;

            return this;
        }

        public GameObject Build() => PlayerObject;
    }
}