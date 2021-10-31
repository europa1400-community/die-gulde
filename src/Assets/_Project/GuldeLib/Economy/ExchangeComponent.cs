using System;
using GuldeLib.Entities;
using GuldeLib.Inventory;
using GuldeLib.Maps;
using GuldeLib.Production;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Economy
{
    [HideMonoScript]
    [RequireComponent(typeof(InventoryComponent))]
    public class ExchangeComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        public bool IsAccepting { get; set; }

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
            Entity && partner.Location && partner.Location.EntityRegistry.IsRegistered(Entity) ||
            Location && partner.Entity && Location.EntityRegistry.IsRegistered(partner.Entity) ||
            !Entity && !Location ||
            !partner.Entity && !partner.Location;

        public float GetPrice(Item item)
        {
            var targetInventory = GetTargetInventory(item);
            var supply = targetInventory.GetSupply(item);
            var supplyDifference = supply - item.MeanSupply;

            return item.MeanPrice - Mathf.Clamp(supplyDifference / (float)item.MeanSupply, -1f, 1f) * (item.MeanPrice - item.MinPrice);
        }

        public InventoryComponent GetTargetInventory(Item item) =>
            item.ItemType == ItemType.Resource || !HasSeperateInventories ? Inventory : ProductInventory;

        public bool CanSellTo(Item item, ExchangeComponent partner)
        {
            var targetInventory = partner.GetTargetInventory(item);

            if (!CanExchangeWith(partner))
            {
                this.Log($"Exchange can't transfer {item}: Can't exchange with partner", LogType.Warning);
                return false;
            }

            if (!(partner.IsAccepting || partner.Owner == Owner))
            {
                this.Log($"Exchange can't transfer {item}: Partner is not accepting sales", LogType.Warning);
                return false;
            }

            if (!targetInventory.CanAddItem(item))
            {
                this.Log($"Exchange can't transfer {item}: Can't add item to partner's inventory", LogType.Warning);
                return false;
            }

            return true;
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
            if (!partner.IsAccepting && partner.Owner != Owner)
            {
                this.Log($"Could not sell {item.Name} to {partner.name}: partner is not accepting.", LogType.Warning);
                return;
            }

            var targetInventory = GetTargetInventory(item);
            if (!targetInventory.HasProductInStock(item, amount)) return;

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

            var targetInventory = partner.GetTargetInventory(item);
            if (!targetInventory.HasProductInStock(item, amount)) return;

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

            ItemBought?.Invoke(this, new ExchangeEventArgs(item, price, amount));
        }

        public void RegisterSale(Item item, float price, int amount = 1)
        {
            this.Log($"Exchange registered sale of {amount} {item} {price * amount} ({price})");

            RemoveItem(item, amount);

            ItemSold?.Invoke(this, new ExchangeEventArgs(item, price, amount));
        }

        public void AddItem(Item item, int amount = 1)
        {
            this.Log($"Exchange added {amount} {item} to inventory");

            var targetInventory = GetTargetInventory(item);
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