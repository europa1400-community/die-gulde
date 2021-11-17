using GuldeLib.Timing;
using UnityEngine;
using Time = GuldeLib.Timing.Time;

namespace GuldeLib.Factories
{
    public class TimeFactory : Factory<Time>
    {
        public TimeFactory(GameObject gameObject, GameObject parentObject) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Time time)
        {
            var timeComponent = GameObject.AddComponent<TimeComponent>();

            timeComponent.Hour = time.Hour;
            timeComponent.Minute = time.Minute;
            timeComponent.Year = time.Year;
            timeComponent.AutoAdvance = time.AutoAdvance;
            timeComponent.EveningHour = time.EveningHour;
            timeComponent.MaxHour = time.MaxHour;
            timeComponent.MinHour = time.MinHour;
            timeComponent.MinYear = time.MinYear;
            timeComponent.MorningHour = time.MorningHour;
            timeComponent.TimeSpeed = time.TimeSpeed;
            timeComponent.NormalTimeSpeed = time.NormalTimeSpeed;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}