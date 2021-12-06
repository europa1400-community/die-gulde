using GuldeLib.Timing;
using UnityEngine;
using Time = GuldeLib.Timing.Time;

namespace GuldeLib.Factories
{
    public class TimeFactory : Factory<Time>
    {
        public TimeFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Time time)
        {
            var timeComponent = GameObject.AddComponent<TimeComponent>();

            timeComponent.Hour = time.Hour.Value;
            timeComponent.Minute = time.Minute.Value;
            timeComponent.Year = time.Year.Value;
            timeComponent.AutoAdvance = time.AutoAdvance;
            timeComponent.EveningHour = time.EveningHour;
            timeComponent.MaxHour = time.MaxHour;
            timeComponent.MinHour = time.MinHour;
            timeComponent.MinYear = time.MinYear;
            timeComponent.MorningHour = time.MorningHour;
            timeComponent.TimeSpeed = time.TimeSpeed.Value;
            timeComponent.NormalTimeSpeed = time.NormalTimeSpeed.Value;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}