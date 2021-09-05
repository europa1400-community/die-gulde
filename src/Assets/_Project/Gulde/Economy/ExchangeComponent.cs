using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Entities;
using Gulde.Inventory;
using MonoLogger.Runtime;
using Gulde.Maps;
using Gulde.Production;
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
        public WealthComponent Owner { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [BoxGroup("Info")]
        public InventoryComponent Inventory { get; private set; }

        [OdinSerialize]
        [ReadOnly]
        [BoxGroup("Info")]
        public InventoryComponent ProductInventory { get; private set; }

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

        [ShowInInspector]
        [BoxGroup("Info")]
        public bool HasSeperateInventories => ProductInventory;

        public event EventHandler<ExchangeEventArgs> ItemSold;
        public event EventHandler<ExchangeEventArgs> ItemBought;

        public bool CanExchangeWith(ExchangeComponent partner) =>
            !Entity || !partner.Location || partner.Location.EntityRegistry.IsRegistered(Entity);

        public float GetPrice(Item item)
        {
            var supply = Inventory.GetSupply(item);
            var supplyDifference = supply - item.MeanSupply;

            return item.MeanPrice - Mathf.Clamp(supplyDifference / (float)item.MeanSupply, -1f, 1f) * (item.MeanPrice - item.MinPrice);
        }

        public InventoryComponent GetTargetInventory(Item item) =>
            item.ItemType == ItemType.Resource || !HasSeperateInventories ? Inventory : ProductInventory;

        public bool CanSellTo(Item item, ExchangeComponent partner)
        {
            var targetInventory = partner.GetTargetInventory(item);
            var canAddToInventory = targetInventory.CanAddItem(item);

            return CanExchangeWith(partner) && (partner.IsAccepting || partner.Owner == Owner) && canAddToInventory;
        }

        void Awake()
        {
            this.Log("Exchange initializing");

            Inventory = GetComponent<InventoryComponent>();
            var inventories = GetComponents<InventoryComponent>();
            if (inventories.Length > 1) ProductInventory = inventories[1];

            Location = GetComponentInParent<LocationComponent>();
            Entity = GetComponent<EntityComponent>();
        }

        public void SellItem(Item item, ExchangeComponent partner, int amount = 1)
        {
            if (!CanExchangeWith(partner)) return;
            if (!partner.IsAccepting && partner.Owner != Owner) return;
            if (!Inventory.HasProductInStock(item, amount)) return;

            var price = partner.GetPrice(item);

            if (Owner == partner.Owner)
            {
                this.Log($"Exchange transfered {amount} {item} to {partner}");

                RemoveItem(item, amount);
                partner.AddItem(item, amount);
            }
            else
            {
                this.Log($"Exchange sold {amount} {item} to {partner} for {price * amount} ({price})");
                
                RegisterSale(item, price, amount);
                partner.RegisterPurchase(item, price, amount);
            }
        }

        public void BuyItem(Item item, ExchangeComponent partner, int amount = 1)
        {
            if (!CanExchangeWith(partner)) return;
            if (!partner.Inventory.HasProductInStock(item, amount)) return;

            var price = partner.GetPrice(item);

            if (Owner == partner.Owner)
            {
                this.Log($"Exchange transfered {amount} {item} from {partner}");

                AddItem(item, amount);
                partner.RemoveItem(item, amount);
            }
            else
            {
                this.Log($"Exchange bought {amount} {item} from {partner} for {price * amount} ({price})");

                RegisterPurchase(item, price, amount);
                partner.RegisterSale(item, price, amount);
            }
        }

        public void RegisterPurchase(Item item, float price, int amount = 1)
        {
            this.Log($"Exchange registered purchase of {amount} {item} {price * amount} ({price})");

            AddItem(item, amount);
            if (Owner) Owner.RemoveMoney(price * amount);

            ItemBought?.Invoke(this, new ExchangeEventArgs(item, price, amount));
        }

        public void RegisterSale(Item item, float price, int amount = 1)
        {
            this.Log($"Exchange registered sale of {amount} {item} {price * amount} ({price})");

            RemoveItem(item, amount);
            if (Owner) Owner.AddMoney(price * amount);

            ItemSold?.Invoke(this, new ExchangeEventArgs(item, price, amount));
        }

        public void AddItem(Item item, int amount = 1)
        {
            this.Log($"Exchange added {amount} {item} to inventory");

            var targetInventory =
                item.ItemType == ItemType.Resource || !HasSeperateInventories ? Inventory : ProductInventory;
            targetInventory.Add(item, amount);
        }

        public void RemoveItem(Item item, int amount = 1)
        {
            this.Log($"Exchange removed {amount} {item} to inventory");

            var targetInventory =
                item.ItemType == ItemType.Resource || !HasSeperateInventories ? Inventory : ProductInventory;
            targetInventory.Remove(item, amount);
        }
    }
}