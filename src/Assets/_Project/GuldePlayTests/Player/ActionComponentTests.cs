using System.Collections;
using GuldeLib;
using GuldeLib.Builders;
using GuldeLib.Player;
using NUnit.Framework;
using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Player
{
    public class ActionComponentTests
    {
        PlayerBuilder PlayerBuilder { get; set; }
        GameBuilder GameBuilder { get; set; }
        CityBuilder CityBuilder { get; set; }

        GameObject PlayerObject => PlayerBuilder.PlayerObject;
        ActionComponent Action => PlayerObject.GetComponent<ActionComponent>();

        int ChangedPoints { get; set; }
        bool PointsChangedFlag { get; set; }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            PlayerBuilder = A.Player;
            CityBuilder = A.City
                .WithSize(10, 10)
                .WithAutoAdvance(true);
            GameBuilder = A.Game
                .WithCity(CityBuilder)
                .WithTimeScale(20f);

            yield return GameBuilder.Build();
            yield return PlayerBuilder.Build();
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            PointsChangedFlag = false;
            ChangedPoints = 0;
        }

        [UnityTest]
        public IEnumerator ShouldAddPointsPerYear()
        {
            Action.PointsChanged += OnPointsChanged;

            Assert.AreEqual(0, Action.Points);
            yield return Locator.Time.WaitForYearTicked;
            Assert.AreEqual(Action.PointsPerRound, Action.Points);
            Assert.True(PointsChangedFlag);
            Assert.AreEqual(4, ChangedPoints);
        }

        void OnPointsChanged(object sender, ActionEventArgs e)
        {
            PointsChangedFlag = true;
            ChangedPoints = e.Points;
        }
    }
}