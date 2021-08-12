using System.Collections.Generic;
using Gulde.Buildings;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Economy
{
    public class WealthComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public float Money { get; private set; }

        [OdinSerialize]
        public List<BuildingComponent> OwnedBuildings { get; } = new List<BuildingComponent>();

        public void AddMoney(float value) => Money += value;

        public void RemoveMoney(float value) => Money -= value;

    }
}