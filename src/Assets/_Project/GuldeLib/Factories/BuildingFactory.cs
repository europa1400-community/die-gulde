using GuldeLib.Maps.Buildings;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class BuildingFactory : Factory<Building, BuildingComponent>
    {
        public BuildingFactory(Building building, GameObject gameObject) : base(building, gameObject, null) { }

        public override BuildingComponent Create()
        {
            Component.Building = TypeObject;
            return Component;
        }
    }
}