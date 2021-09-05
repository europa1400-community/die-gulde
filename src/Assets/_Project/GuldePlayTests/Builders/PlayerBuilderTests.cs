using System.Collections;
using Gulde.Builders;
using Gulde.Economy;
using Gulde.Inventory;
using Gulde.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Builders
{
    public class PlayerBuilderTests
    {
        PlayerBuilder PlayerBuilder { get; set; }
        GameObject PlayerObject => PlayerBuilder.PlayerObject;
        InventoryComponent Inventory => PlayerObject.GetComponent<InventoryComponent>();
        PlayerComponent Player => PlayerObject.GetComponent<PlayerComponent>();
        WealthComponent Wealth => PlayerObject.GetComponent<WealthComponent>();

        [SetUp]
        public void Setup()
        {
            PlayerBuilder = A.Player;
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            PlayerBuilder = null;
        }

        [UnityTest]
        public IEnumerator ShouldBuildPlayer()
        {
            yield return PlayerBuilder.Build();

            Assert.NotNull(PlayerObject);
            Assert.NotNull(Inventory);
            Assert.NotNull(Player);
            Assert.NotNull(Wealth);
        }

        [UnityTest]
        public IEnumerator ShouldBuildPlayerWithSlots()
        {
            yield return PlayerBuilder.WithSlots(3).Build();

            Assert.NotNull(PlayerObject);
            Assert.AreEqual(3, Inventory.Slots);
        }
    }
}