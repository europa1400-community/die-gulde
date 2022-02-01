using GuldeLib.Cities;
using GuldeLib.Economy;
using GuldeLib.Maps;
using GuldeLib.Players;
using GuldeLib.Timing;

namespace GuldeLib
{
    public static class Locator
    {
        public static TimeComponent Time { get; set; }
        public static CitizenComponent Citizen { get; set; }
        public static MarketComponent Market { get; set; }
        public static CityComponent City { get; set; }
    }
}