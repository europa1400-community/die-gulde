using Gulde.Pathfinding;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Population
{
    [RequireComponent(typeof(NavComponent))]
    public class MapComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ReadOnly]
        public NavComponent NavComponent { get; private set; }

        void Awake()
        {
            NavComponent = GetComponent<NavComponent>();
        }
    }
}