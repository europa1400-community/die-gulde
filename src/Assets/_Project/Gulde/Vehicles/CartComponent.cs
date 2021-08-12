using Gulde.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gulde.Vehicles
{
    [RequireComponent(typeof(InventoryComponent))]
    public class CartComponent : SerializedMonoBehaviour
    {
        InventoryComponent InventoryComponent { get; set; }

        void Awake()
        {
            InventoryComponent = GetComponent<InventoryComponent>();
        }
    }
}