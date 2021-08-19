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
        [BoxGroup("Settings")]
        public bool IsAccepting { get; private set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        public WealthComponent Owner { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [BoxGroup("Info")]
        public InventoryComponent Inventory { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [ShowIf("Location")]
        [BoxGroup("Info")]
        public LocationComponent Location { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [ShowIf("Entity")]
        [BoxGroup("Info")]
        public EntityComponent Entity { get; private set; }

        public event EventHandler<ExchangeEventArgs> ItemSold;
        public event EventHandler<ExchangeEventArgs> ItemBought;

        public bool CanExchangeWith(ExchangeComponent partner) =>
            !Entity || !partner.Location || partner.Location.EntityRegistry.IsRegistered(Entity);

        public float Price(Item item)
        {
            var supply = Inventory.GetSupply(item);
            var supplyDifference = supply - item.MeanSupply;

            return item.MeanPrice + supplyDifference / item.SupplyWeight;
        }

        void Awake()
        {
            Inventory = GetComponent<InventoryComponent>();
            Location = GetComponentInParent<LocationComponent>();
            Entity = GetComponent<EntityComponent>();
        }

        public void SellItem(Item item, ExchangeComponent partner)
        {
            if (!CanExchangeWith(partner)) return;
            if (!partner.IsAccepting) return;
            if (!Inventory.HasProductInStock(item)) return;

            var price = partner.Price(item);

            if (Owner == partner.Owner)
            {
                Inventory.Remove(item);
                partner.Inventory.Add(item);
            }
            else
            {
                RegisterSale(item, price);
                partner.RegisterPurchase(item, price);
            }

            Debug.Log($"{name} sold {item.Name} to {partner.name} for {price}");
        }

        public void BuyItem(Item item, ExchangeComponent partner)
        {
            if (!CanExchangeWith(partner)) return;
            if (!partner.Inventory.HasProductInStock(item)) return;

            var price = partner.Price(item);

            if (Owner == partner.Owner)
            {
                Inventory.Add(item);
                partner.Inventory.Remove(item);
            }
            else
            {
                RegisterPurchase(item, price);
                partner.RegisterSale(item, price);
            }

            Debug.Log($"{name} bought {item.Name} from {partner.name} for {price}");
        }

        public void RegisterPurchase(Item item, float price)
        {
            Inventory.Add(item);
            if (Owner) Owner.RemoveMoney(price);

            ItemBought?.Invoke(this, new ExchangeEventArgs(item, price));
        }

        public void RegisterSale(Item item, float price)
        {
            Inventory.Remove(item);
            if (Owner) Owner.AddMoney(price);

            ItemSold?.Invoke(this, new ExchangeEventArgs(item, price));
        }
    }
}