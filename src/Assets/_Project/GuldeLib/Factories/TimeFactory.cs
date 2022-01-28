using GuldeLib.Timing;
using UnityEngine;
using Time = GuldeLib.TypeObjects.Time;

namespace GuldeLib.Factories
{
    public class TimeFactory : Factory<Time, TimeComponent>
    {
        public TimeFactory(Time time, GameObject gameObject = null, GameObject parentObject = null) : base(time, gameObject, parentObject)
        {
        }

        public override TimeComponent Create()
        {
            Component.Hour = TypeObject.Hour.Value;
            Component.Minute = TypeObject.Minute.Value;
            Component.Year = TypeObject.Year.Value;
            Component.AutoAdvance = TypeObject.AutoAdvance;
            Component.EveningHour = TypeObject.EveningHour;
            Component.MaxHour = TypeObject.MaxHour;
            Component.MinHour = TypeObject.MinHour;
            Component.MinYear = TypeObject.MinYear;
            Component.MorningHour = TypeObject.MorningHour;
            Component.TimeSpeed = TypeObject.TimeSpeed.Value;
            Component.NormalTimeSpeed = TypeObject.NormalTimeSpeed.Value;

            Locator.Time = Component;

            return Component;
        }
    }
}