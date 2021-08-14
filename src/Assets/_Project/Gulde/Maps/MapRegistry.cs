using System.Collections.Generic;

namespace Gulde.Maps
{
    public static class MapRegistry
    {
        public static HashSet<MapComponent> Maps { get; } = new HashSet<MapComponent>();

        public static void RegisterMap(MapComponent mapComponent)
        {
            Maps.Add(mapComponent);
        }

        public static void UnregisterMap(MapComponent mapComponent)
        {
            if (!Maps.Contains(mapComponent)) return;
            Maps.Remove(mapComponent);
        }
    }
}