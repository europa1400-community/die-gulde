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
            Debug.Log("create Building");
            Component.Building = TypeObject;
            Debug.Log("Component: " + Component);
            return Component;
        }
    }
}