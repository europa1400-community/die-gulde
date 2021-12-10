using GuldeLib.Maps.Buildings;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class BuildingFactory : Factory<Building, BuildingComponent>
    {
        public BuildingFactory(GameObject gameObject) : base(gameObject, null) { }

        public override BuildingComponent Create(Building building)
        {
            Component.Building = building;
            return Component;
        }
    }
}