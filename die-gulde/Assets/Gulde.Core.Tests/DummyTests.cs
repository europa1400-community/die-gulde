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
            Assert.True(gameObject.GetComponent<GameComponent>());
        }
    }
}
