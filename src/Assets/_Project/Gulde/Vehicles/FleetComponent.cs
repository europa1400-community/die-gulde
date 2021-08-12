using System.Collections.Generic;
using Gulde.Inventory;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Vehicles
{
    [RequireComponent(typeof(CartComponent))]
    public class FleetComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        List<CartComponent> Carts { get; set; } = new List<CartComponent>();
    }
}