// using System;
// using System.Collections;
// using GuldeLib.Builders;
// using GuldeLib.Timing;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
//
// namespace GuldePlayTests.Timing
// {
//     public class TimeComponentTests
//     {
//         GameBuilder GameBuilder { get; set; }
//         CityBuilder CityBuilder { get; set; }
//
//         GameObject CityObject => CityBuilder.CityObject;
//         TimeComponent Time => CityObject.GetComponent<TimeComponent>();
//
//         bool YearTickedFlag { get; set; }
//         bool WorkingHourTickedFlag { get; set; }
//         int TickedWorkingMinute { get; set; }
//         int TickedWorkingHour { get; set; }
//         int TickedWorkingYear { get; set; }
//         bool MorningFlag { get; set; }
//         bool EveningFlag { get; set; }
//
//         [UnitySetUp]
//         public IEnumerator Setup()
//         {
//             CityBuilder = A.City.WithSize(10, 10).WithNormalTimeSpeed(500);
//             GameBuilder = A.Game.WithCity(CityBuilder).WithTimeScale(10f);
//
//             yield return GameBuilder.Build();
//         }
//
//         [TearDown]
//         public void Teardown()
//         {
//             WorkingHourTickedFlag = false;
//             YearTickedFlag = false;
//             MorningFlag = false;
//             EveningFlag = false;
//             TickedWorkingMinute = 0;
//             TickedWorkingHour = 0;
//             TickedWorkingYear = 0;
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldStartAndStopTime()
//         {
//             CityBuilder = CityBuilder.WithNormalTimeSpeed(5);
//             yield return GameBuilder.WithTimeScale(1f).Build();
//
//             Assert.True(Time.IsRunning);
//             Assert.AreEqual(Time.NormalTimeSpeed, Time.TimeSpeed);
//             Assert.AreEqual(1f, Time.TimeScale);
//
//             Time.StopTime();
//             Assert.False(Time.IsRunning);
//
//             Time.StartTime();
//             Assert.True(Time.IsRunning);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldToggleTime()
//         {
//             Assert.True(Time.IsRunning);
//
//             Time.ToggleTime();
//             Assert.False(Time.IsRunning);
//
//             Time.ToggleTime();
//             Assert.True(Time.IsRunning);
//
//             yield return null;
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldResetTime()
//         {
//             yield return Time.WaitForWorkingHourTicked;
//
//             Time.ResetTime();
//
//             Assert.AreEqual(0, Time.Minute);
//             Assert.AreEqual(Time.MinHour, Time.Hour);
//             Assert.AreEqual(Time.MinYear, Time.Year);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldTickWorkingHour()
//         {
//             Time.WorkingHourTicked += OnWorkingHourTicked;
//
//             yield return Time.WaitForWorkingHourTicked;
//
//             Assert.True(WorkingHourTickedFlag);
//             Assert.True(Time.IsWorkingHour);
//             Assert.AreEqual(Time.MorningHour, Time.Hour);
//             Assert.AreEqual(Time.MinYear, Time.Year);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldAutoAdvance()
//         {
//             CityBuilder = CityBuilder.WithAutoAdvance(true);
//             yield return GameBuilder.Build();
//
//             Time.YearTicked += OnYearTicked;
//
//             yield return Time.WaitForYearTicked;
//
//             Assert.True(YearTickedFlag);
//             Assert.True(Time.IsRunning);
//
//             YearTickedFlag = false;
//
//             yield return Time.WaitForYearTicked;
//
//             Assert.True(YearTickedFlag);
//             Assert.True(Time.IsRunning);
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldTickMorning()
//         {
//             Time.Morning += OnMorningTicked;
//
//             yield return Time.WaitForMorning;
//
//             Assert.True(MorningFlag);
//             Assert.AreEqual(Time.MorningHour, Time.Hour);
//         }
//
//         void OnMorningTicked(object sender, EventArgs e)
//         {
//             MorningFlag = true;
//         }
//
//         [UnityTest]
//         public IEnumerator ShouldTickEvening()
//         {
//             Time.Evening += OnEveningTicked;
//
//             yield return Time.WaitForEvening;
//
//             Assert.True(EveningFlag);
//             Assert.AreEqual(Time.EveningHour, Time.Hour);
//         }
//
//         void OnEveningTicked(object sender, EventArgs e)
//         {
//             EveningFlag = true;
//         }
//
//         void OnYearTicked(object sender, TimeEventArgs e)
//         {
//             YearTickedFlag = true;
//         }
//
//         void OnWorkingHourTicked(object sender, TimeEventArgs e)
//         {
//             WorkingHourTickedFlag = true;
//             TickedWorkingMinute = e.Minute;
//             TickedWorkingHour = e.Hour;
//             TickedWorkingYear = e.Year;
//         }
//     }
// }