using GuldeLib.TypeObjects;
using Sirenix.OdinInspector;

namespace GuldeLib.Maps.Buildings
{
    public class BuildingComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        public Building Building { get; set; }
    }
}