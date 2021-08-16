using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Entities;
using Gulde.Inventory;
using Gulde.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Economy
{
    [HideMonoScript]
    [RequireComponent(typeof(InventoryComponent))]
    public class ExchangeComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ReadOnly]
        public InventoryComponent Inventory { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [ShowIf("Wealth")]
        public WealthComponent Wealth { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [ShowIf("Location")]
        public LocationComponent Location { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [ShowIf("Entity")]
        public EntityComponent Entity { get; private set; }

        void Awake()
        {
            Inventory = GetComponent<InventoryComponent>();
            Wealth = GetComponent<WealthComponent>();
            Location = GetComponentInParent<LocationComponent>();
            Entity = GetComponent<EntityComponent>();
        }

        public void SellProduct(Item item, ExchangeComponent partner)
        {
            if (!CanExchangeWith(partner))
            {
                Debug.Log($"{partner} can't exchange with {name}");
                return;
            }
            if (!Inventory.HasProductInStock(item)) return;

            var price = partner.GetPrice(item);

            Inventory.Remove(item);
            if (Wealth) Wealth.AddMoney(price);

            partner.Inventory.Add(item);
            if (partner.Wealth) partner.Wealth.RemoveMoney(price);

            Debug.Log($"{name} sold {item.Name} to {partner.name} for {price}");
        }

        public void BuyProduct(Item item, ExchangeComponent partner)
        {
            if (!CanExchangeWith(partner))
            {
                Debug.Log($"{partner} can't exchange with {name}");
                return;
            }
            if (!partner.Inventory.HasProductInStock(item)) return;

            var price = partner.GetPrice(item);

            Inventory.Add(item);
            if (Wealth) Wealth.RemoveMoney(price);

            partner.Inventory.Remove(item);
            if (partner.Wealth) partner.Wealth.AddMoney(price);

            Debug.Log($"{name} bought {item.Name} from {partner.name} for {price}");
        }

        public float GetPrice(Item item)
        {
            var supply = Inventory.GetSupply(item);
            var supplyDifference = supply - item.MeanSupply;

            return item.MeanPrice + supplyDifference / item.SupplyWeight;
        }

        public bool CanExchangeWith(ExchangeComponent partner) =>
            !Entity || !partner.Location || partner.Location.EntityRegistry.IsRegistered(Entity);

        #region OdinInspector

        void OnValidate()
        {
            Inventory = GetComponent<InventoryComponent>();
            Wealth = GetComponent<WealthComponent>();
            Location = GetComponentInParent<LocationComponent>();
            Entity = GetComponent<EntityComponent>();
        }

        #endregion
    }
}