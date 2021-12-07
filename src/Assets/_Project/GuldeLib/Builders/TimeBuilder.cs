using GuldeLib.Timing;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class TimeBuilder : Builder<Time>
    {
        public TimeBuilder WithNormalTimeSpeed(float normalTimeSpeed)
        {
            Object.NormalTimeSpeed = normalTimeSpeed;
            return this;
        }

        public TimeBuilder WithMinYear(int minYear)
        {
            Object.MinYear = minYear;
            return this;
        }

        public TimeBuilder WithMinHour(int minHour)
        {
            Object.MinHour = minHour;
            return this;
        }

        public TimeBuilder WithMorningHour(int morningHour)
        {
            Object.MorningHour = morningHour;
            return this;
        }

        public TimeBuilder WithEveningHour(int eveningHour)
        {
            Object.EveningHour = eveningHour;
            return this;
        }

        public TimeBuilder WithMaxHour(int maxHour)
        {
            Object.MaxHour = maxHour;
            return this;
        }

        public TimeBuilder WithMinute(int minute)
        {
            Object.Minute = minute;
            return this;
        }

        public TimeBuilder WithHour(int hour)
        {
            Object.Hour = hour;
            return this;
        }

        public TimeBuilder WithYear(int year)
        {
            Object.Year = year;
            return this;
        }

        public TimeBuilder WithTimeSpeed(float timeSpeed)
        {
            Object.TimeSpeed = timeSpeed;
            return this;
        }

        public TimeBuilder WithAutoAdvance(bool autoAdvance)
        {
            Object.AutoAdvance = autoAdvance;
            return this;
        }
    }
}