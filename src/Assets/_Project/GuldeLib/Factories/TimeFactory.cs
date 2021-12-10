using GuldeLib.Timing;
using UnityEngine;
using Time = GuldeLib.TypeObjects.Time;

namespace GuldeLib.Factories
{
    public class TimeFactory : Factory<Time, TimeComponent>
    {
        public TimeFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override TimeComponent Create(Time time)
        {
            Component.Hour = time.Hour.Value;
            Component.Minute = time.Minute.Value;
            Component.Year = time.Year.Value;
            Component.AutoAdvance = time.AutoAdvance;
            Component.EveningHour = time.EveningHour;
            Component.MaxHour = time.MaxHour;
            Component.MinHour = time.MinHour;
            Component.MinYear = time.MinYear;
            Component.MorningHour = time.MorningHour;
            Component.TimeSpeed = time.TimeSpeed.Value;
            Component.NormalTimeSpeed = time.NormalTimeSpeed.Value;

            Locator.Time = Component;

            return Component;
        }
    }
}