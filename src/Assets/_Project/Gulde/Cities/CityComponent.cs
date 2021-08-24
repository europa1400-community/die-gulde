using Gulde.Maps;
using Gulde.Timing;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Cities
{
    [RequireComponent(typeof(MapComponent))]
    [RequireComponent(typeof(TimeComponent))]
    public class CityComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public MapComponent Map { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public TimeComponent Time { get; private set; }

        void Awake()
        {
            Map = GetComponent<MapComponent>();
            Time = GetComponent<TimeComponent>();

            Locator.City = this;
        }
    }
}