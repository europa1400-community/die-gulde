using System.Collections;
using GuldeLib.Builders;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace GuldePlayTests.Builders
{
    public class GameBuilderTests
    {
        GameBuilder GameBuilder { get; set; }
        CityBuilder CityBuilder { get; set; }

        [SetUp]
        public void Setup()
        {
            CityBuilder = A.City.WithSize(20, 20);
            GameBuilder = A.Game.WithCity(CityBuilder);
        }

        [TearDown]
        public void Teardown()
        {

        }

        [UnityTest]
        public IEnumerator ShouldBuildGame()
        {
            yield return GameBuilder.Build();

            var activeScene = SceneManager.GetActiveScene();
            Assert.AreEqual("game", activeScene.name);
        }

        [UnityTest]
        public IEnumerator ShouldBuildGameWithTimeScale()
        {
            yield return GameBuilder.WithTimeScale(2f).Build();

            Assert.AreEqual(2f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator ShouldBuildGameWithoutTimeScale()
        {
            yield return GameBuilder.Build();

            Assert.AreEqual(1f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator ShouldBuildGameWithSceneName()
        {
            yield return GameBuilder.WithSceneName("some-name").Build();

            var activeScene = SceneManager.GetActiveScene();
            Assert.AreEqual("some-name", activeScene.name);
        }
    }
}
