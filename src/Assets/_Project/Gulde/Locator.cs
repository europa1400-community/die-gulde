using Gulde.Cities;
using Gulde.Company;
using Gulde.Economy;
using Gulde.Maps;
using Gulde.Player;
using Gulde.Timing;

namespace Gulde
{
    public static class Locator
    {
        public static MapSelectorComponent MapSelector { get; set; }
        public static TimeComponent Time { get; set; }
        public static PlayerComponent Player { get; set; }
        public static MarketComponent Market { get; set; }
        public static CityComponent City { get; set; }
    }
}