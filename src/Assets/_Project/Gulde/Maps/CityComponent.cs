using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Maps
{
    [RequireComponent(typeof(MapComponent))]
    public class CityComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public MapComponent Map { get; private set; }

        void Awake()
        {
            Locator.City = this;
            Map = GetComponent<MapComponent>();
        }
    }
}