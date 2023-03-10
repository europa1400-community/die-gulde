using NUnit.Framework;
using UnityEngine;

namespace Gulde.Core.Tests
{
    public class DummyTests
    {
        [Test]
        public void ShouldPass()
        {
            Assert.Pass();
        }

        [Test]
        public void ShouldAddGameComponent()
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<GameComponent>();
            var gameComponent = gameObject.GetComponent<GameComponent>();
            gameComponent.Start();
            Assert.NotNull(gameComponent);
        }
    }
}
