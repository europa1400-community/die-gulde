using Gulde.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Cities
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
            Map = GetComponent<MapComponent>();

            Locator.City = this;
        }
    }
}