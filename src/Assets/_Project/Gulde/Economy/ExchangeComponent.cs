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

        public void SellProduct(Product product, ExchangeComponent partner)
        {
            if (!CanExchangeWith(partner))
            {
                Debug.Log($"{partner} can't exchange with {name}");
                return;
            }
            if (!Inventory.HasProductInStock(product)) return;

            var price = partner.GetPrice(product);

            Inventory.RemoveProduct(product);
            if (Wealth) Wealth.AddMoney(price);

            partner.Inventory.AddProduct(product);
            if (partner.Wealth) partner.Wealth.RemoveMoney(price);

            Debug.Log($"{name} sold {product.Name} to {partner.name} for {price}");
        }

        public void BuyProduct(Product product, ExchangeComponent partner)
        {
            if (!CanExchangeWith(partner))
            {
                Debug.Log($"{partner} can't exchange with {name}");
                return;
            }
            if (!partner.Inventory.HasProductInStock(product)) return;

            var price = partner.GetPrice(product);

            Inventory.AddProduct(product);
            if (Wealth) Wealth.RemoveMoney(price);

            partner.Inventory.RemoveProduct(product);
            if (partner.Wealth) partner.Wealth.AddMoney(price);

            Debug.Log($"{name} bought {product.Name} from {partner.name} for {price}");
        }

        public float GetPrice(Product product)
        {
            var supply = Inventory.GetSupply(product);
            var supplyDifference = supply - product.MeanSupply;

            return product.MeanPrice + supplyDifference / product.SupplyWeight;
        }

        public bool CanExchangeWith(ExchangeComponent partner) =>
            !Entity || !partner.Location || partner.Location.IsEntityRegistered(Entity);

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