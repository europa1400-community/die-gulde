using Gulde.Buildings;
using Gulde.Entities;
using Gulde.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Company.Employees
{
    [RequireComponent(typeof(LocationComponent))]
    public class WorkerHomeComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public LocationComponent Location { get; private set; }

        void Awake()
        {
            Location = GetComponent<LocationComponent>();

            Location.ContainingMapChanged += OnContainingMapChanged;
        }

        void OnContainingMapChanged(object sender, MapEventArgs e)
        {
            Location.ContainingMap.WorkerHomes.Add(this);
        }
    }
}