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
    /// <summary>
    /// Provides functionality for economic and non-economic item exchanges.
    /// </summary>
    [RequireComponent(typeof(InventoryComponent))]
    public class ExchangeComponent : SerializedMonoBehaviour
    {
        /// <summary>
        /// Gets or sets whether the <see cref = "ExchangeComponent">ExchangeComponent</see> will automatically purchase items.
        /// </summary>
        /// <remarks>
        /// This is used to prevent item sales to ExchangeComponents that don't generally accept anything.
        /// An example for this would be companies and player inventories.
        /// </remarks>
        [ShowInInspector]
        [BoxGroup("Settings")]
        public bool IsPurchasing { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref = "ExchangeComponent">ExchangeComponent</see> will automatically sell items.
        /// </summary>
        /// <remarks>
        /// This is used to prevent item purchases from ExchangeComponents that don't generally sell anything.
        /// An example for this would be companies and player inventories.
        /// </remarks>
        [ShowInInspector]
        [BoxGroup("Settings")]
        public bool IsSelling { get; set; }

        /// <summary>
        /// Gets or sets the owner <see cref = "WealthComponent">WealthComponent</see> of the <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// Any item exchanges will be billed to this WealthComponent.
        /// If no owner is provided, the ExchangeComponent has an infinite supply of money.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Settings")]
        public WealthComponent Owner { get; set; }

        /// <summary>
        /// Gets the default <see cref = "InventoryComponent">InventoryComponent</see> the <see cref = "ExchangeComponent">ExchangeComponent</see> uses to exchange any <see cref = "Item">Items</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [BoxGroup("Info")]
        public InventoryComponent Inventory { get; private set; }

        /// <summary>
        /// Gets the optional <see cref = "InventoryComponent">InventoryComponent</see> the <see cref = "ExchangeComponent">ExchangeComponent</see> uses to exchange <see cref = "Item">Items</see> produced by <see cref = "Recipe">Recipes</see>.
        /// If provided, the <see cref = "ExchangeComponent.Inventory">Inventory</see> will only be used to exchange resource <see cref = "ItemType">type</see> <see cref = "Item">Items</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [BoxGroup("Info")]
        public InventoryComponent ProductInventory { get; private set; }

        /// <summary>
        /// Gets the <see cref = "LocationComponent">LocationComponent</see> associated to this <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [ShowIf("Location")]
        [BoxGroup("Info")]
        public LocationComponent Location { get; private set; }

        /// <summary>
        /// Gets the <see cref = "EntityComponent">EntityComponent</see> associated to this <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// </summary>
        [ShowInInspector]
        [ReadOnly]
        [ShowIf("Entity")]
        [BoxGroup("Info")]
        public EntityComponent Entity { get; private set; }

        /// <summary>
        /// Gets whether this <see cref = "ExchangeComponent">ExchangeComponent</see> uses different <see cref = "InventoryComponent">InventoryComponents</see> for resource <see cref = "ItemType">type</see> and product type <see cref = "Item">Items</see>.
        /// </summary>
        [ShowInInspector]
        [BoxGroup("Info")]
        public bool HasSeperateInventories => ProductInventory;

        /// <summary>
        /// Invoked after an <see cref = "Item">Item</see> has been sold by the <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// Note: This is not invoked when two <see cref = "ExchangeComponent">ExchangeComponents</see> with the same owner exchange items.
        /// </summary>
        public event EventHandler<ItemSoldEventArgs> ItemSold;

        /// <summary>
        /// Invoked after an <see cref = "Item">Item</see> has been bought by the <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// Note: This is not invoked when two <see cref = "ExchangeComponent">ExchangeComponents</see> with the same owner exchange items.
        /// </summary>
        public event EventHandler<ItemBoughtEventArgs> ItemBought;

        /// <summary>
        /// Gets whether this <see cref = "ExchangeComponent">ExchangeComponent</see> is able to exchange items with the provided other ExchangeComponent.
        /// </summary>
        /// <remarks>
        /// To be able to exchange with each other, either one ExchangeComponent needs to have an associated <see cref = "ExchangeComponent.Location">Location</see>
        /// with the other ExchangeComponent's <see cref = "ExchangeComponent.Entity">Entity</see> being registered at this Location,
        /// or one or both of the ExchangeComponents need to not have an associated Location and Entity at all.
        /// (The latter would be the case when a player exchanges with a company or market.)
        /// </remarks>
        /// <param name="other">The other ExchangeComponent to check against.</param>
        public bool CanExchangeWith(ExchangeComponent other) =>
            Entity && other.Location && other.Location.EntityRegistry.IsRegistered(Entity) ||
            Location && other.Entity && Location.EntityRegistry.IsRegistered(other.Entity) ||
            !Entity && !Location ||
            !other.Entity && !other.Location;


        /// <summary>
        /// Gets the price associated to the given <see cref = "Item">Item</see> by this <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// The price will be changing according to supply and demand.
        /// </summary>
        /// <param name="item">The item to check.</param>
        public float GetPrice(Item item)
        {
            var targetInventory = GetTargetInventory(item);
            var supply = targetInventory.GetSupply(item);
            var supplyDifference = supply - item.MeanSupply;

            return item.MeanPrice - Mathf.Clamp(supplyDifference / (float)item.MeanSupply, -1f, 1f) * (item.MeanPrice - item.MinPrice);
        }

        /// <summary>
        /// Gets the <see cref = "InventoryComponent">InventoryComponent</see> the given <see cref = "Item">Item</see> that will be used for the exchange by this <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// </summary>
        /// <remarks>
        /// The result will either be the <see cref = "ExchangeComponent.Inventory">Inventory</see> or <see cref = "ExchangeComponent.ProductInventory">ProductInventory</see>
        /// depending on the <see cref="ItemType"/> and whether a ProductInventory is specified in this ExchangeComponent.
        /// </remarks>
        /// <param name="item">The item to check.</param>
        /// <returns></returns>
        public InventoryComponent GetTargetInventory(Item item) =>
            item.ItemType == ItemType.Resource || !HasSeperateInventories ? Inventory : ProductInventory;

        /// <summary>
        /// Gets whether this <see cref = "ExchangeComponent">ExchangeComponent</see> is able to sell a given amount
        /// of a given <see cref = "Item">Item</see> to the provided other ExchangeComponent.
        /// </summary>
        /// <remarks>
        /// This depends on whether this ExchangeComponent <see cref = "ExchangeComponent.CanExchangeWith">can exchange with</see> the other ExchangeComponent,
        /// whether the other ExchangeComponent <see cref = "ExchangeComponent.IsPurchasing">is generally purchasing</see> (or this is a same-owner exchange),
        /// whether this ExchangeComponent's <see cref = "ExchangeComponent.GetTargetInventory">target inventory</see>
        /// has the requested amount of Items <see cref = "InventoryComponent.HasItemInStock">in stock</see>
        /// and whether the other ExchangeComponent's target inventory
        /// <see cref = "InventoryComponent.CanRegisterItem">is able to register</see> the given Item.
        /// </remarks>
        /// <param name="item">The item to check.</param>
        /// <param name="other">The other ExchangeComponent to check against.</param>
        /// <param name="amount">The amount to check.</param>
        public bool CanSellTo(Item item, ExchangeComponent other, int amount = 1)
        {
            if (!CanExchangeWith(other))
            {
                this.Log($"Exchange can't sell {amount} {item} to {other}: Can't exchange with partner.", LogType.Warning);
                return false;
            }

            if (!(other.IsPurchasing || other.Owner == Owner))
            {
                this.Log($"Exchange can't sell {amount} {item} to {other}: Other is not purchasing.", LogType.Warning);
                return false;
            }

            var targetInventory = GetTargetInventory(item);
            if (!targetInventory.HasItemInStock(item, amount))
            {
                this.Log($"Exchange can't sell {amount} {item} to {other}: Item is not in stock.", LogType.Warning);
                return false;
            }

            var otherTargetInventory = other.GetTargetInventory(item);
            if (!otherTargetInventory.CanRegisterItem(item))
            {
                this.Log($"Exchange can't transfer {item}: Can't add item to other's inventory", LogType.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets whether this <see cref = "ExchangeComponent">ExchangeComponent</see> is able to purchase a given amount
        /// of a given <see cref = "Item">Item</see> from the provided other ExchangeComponent.
        /// </summary>
        /// <remarks>
        /// This depends on whether this ExchangeComponent <see cref = "ExchangeComponent.CanExchangeWith">can exchange with</see> the other ExchangeComponent,
        /// whether the other ExchangeComponent <see cref = "ExchangeComponent.IsSelling">is generally selling</see> (or this is a same-owner exchange),
        /// whether this ExchangeComponent's <see cref = "ExchangeComponent.GetTargetInventory">target inventory</see>
        /// <see cref = "InventoryComponent.CanRegisterItem">is able to register</see> the given item
        /// and whether the other ExchangeComponent's target inventory
        /// has the requested amount of Items <see cref = "InventoryComponent.HasItemInStock">in stock</see>.
        /// </remarks>
        /// <param name="item">The item to check.</param>
        /// <param name="other">The other ExchangeComponent to check against.</param>
        /// <param name="amount">The amount to check.</param>
        public bool CanPurchaseFrom(Item item, ExchangeComponent other, int amount = 1)
        {
            if (!CanExchangeWith(other))
            {
                this.Log($"Exchange can't purchase {amount} {item} from {other}: Can't exchange with other",
                    LogType.Warning);
                return false;
            }

            var targetInventory = GetTargetInventory(item);
            if (!targetInventory.CanRegisterItem(item))
            {
                this.Log($"Exchange can't purchase {amount} {item} from {other}: Can't add item to inventory.", LogType.Warning);
                return false;
            }

            var otherTargetInventory = other.GetTargetInventory(item);
            if (!otherTargetInventory.HasItemInStock(item, amount))
            {
                this.Log($"Exchange can't purchase {amount} {item} from {other}: Other does not have item in stock", LogType.Warning);
                return false;
            }

            return true;
        }

        /// <remarks>
        /// Initializes references to <see cref = "ExchangeComponent.Inventory">Inventory</see>, <see cref = "ExchangeComponent.ProductInventory">ProductInventory</see>,
        /// <see cref = "ExchangeComponent.Location">Location</see> and <see cref = "ExchangeComponent.Entity">Entity</see>.
        /// </remarks>
        void Awake()
        {
            this.Log("Exchange initializing");

            Inventory = GetComponent<InventoryComponent>();
            var inventories = GetComponents<InventoryComponent>();
            if (inventories.Length > 1) ProductInventory = inventories[1];

            Location = GetComponentInParent<LocationComponent>();
            Entity = GetComponent<EntityComponent>();
        }

        /// <summary>
        /// Performs a sale of a given amount of a given <see cref = "Item">Item</see> to the given other <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// </summary>
        /// <remarks>
        /// This will check whether this ExchangeComponent <see cref = "ExchangeComponent.CanSellTo">can sell to</see> the other ExchangeComponent.
        /// If so, then either a same-owner exchange will be performed, which does not bill to the <see cref = "ExchangeComponent.Owner">Owner</see>,
        /// or a normal exchange will be performed, which will bill to the Owner and affect both ExchangeComponent's Owner's money.
        /// </remarks>
        /// <param name="item">The Item to sell.</param>
        /// <param name="other">The other ExchangeComponent to sell to.</param>
        /// <param name="amount">The amount of Items to sell.</param>
        public void Sell(Item item, ExchangeComponent other, int amount = 1)
        {
            if (!CanSellTo(item, other, amount))
            {
                this.Log($"Could not sell {amount} {item} to {other}.", LogType.Warning);
                return;
            }

            if (Owner == other.Owner)
            {
                this.Log($"Exchange transfered {amount} {item} to {other}");

                RemoveItem(item, amount);
                other.AddItem(item, amount);
            }
            else
            {
                var price = other.GetPrice(item);

                this.Log($"Exchange sold {amount} {item} to {other} for {price * amount} ({price})");
                
                RegisterSale(item, price, amount);
                other.RegisterPurchase(item, price, amount);
            }
        }

        /// <summary>
        /// Performs a purchase of a given amount of a given <see cref = "Item">Item</see> from the given other <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// </summary>
        /// <remarks>
        /// This will check whether this ExchangeComponent <see cref = "CanPurchaseFrom">can buy from</see> the other ExchangeComponent.
        /// If so, then either a same-owner exchange will be performed, which does not bill to the <see cref = "ExchangeComponent.Owner">Owner</see>,
        /// or a normal exchange will be performed, which will bill to the Owner and affect both ExchangeComponent's Owner's money.
        /// </remarks>
        /// <param name="item">The Item to purchase.</param>
        /// <param name="other">The other ExchangeComponent to purchase from.</param>
        /// <param name="amount">The amount of Items to purchase.</param>
        public void Purchase(Item item, ExchangeComponent other, int amount = 1)
        {
            if (!CanExchangeWith(other)) return;

            var targetInventory = other.GetTargetInventory(item);
            if (!targetInventory.HasItemInStock(item, amount)) return;

            var price = other.GetPrice(item);

            if (Owner == other.Owner)
            {
                this.Log($"Exchange transfered {amount} {item} from {other}");

                AddItem(item, amount);
                other.RemoveItem(item, amount);
            }
            else
            {
                this.Log($"Exchange purchased {amount} {item} from {other} for {price * amount} ({price})");

                RegisterPurchase(item, price, amount);
                other.RegisterSale(item, price, amount);
            }
        }

        /// <summary>
        /// Executes a single sided purchase of a given amount of a given <see cref = "Item">Item</see> for a given price.
        /// </summary>
        /// <remarks>
        /// This is where the exchanged amount of Items will be <see cref = "ExchangeComponent.AddItem">added</see>
        /// and the exchange will be billed for the previously calculated price.
        /// </remarks>
        /// <param name="item">The item to purchase.</param>
        /// <param name="price">The price the item is purchased for.</param>
        /// <param name="amount">The amount of items being purchased.</param>
        public void RegisterPurchase(Item item, float price, int amount = 1)
        {
            this.Log($"Exchange registered purchase of {amount} {item} {price * amount} ({price})");

            AddItem(item, amount);

            ItemBought?.Invoke(this, new ItemBoughtEventArgs(item, price, amount));
        }

        /// <summary>
        /// Executes a single sided sale of a given amount of a given <see cref = "Item">Item</see> for a given price.
        /// </summary>
        /// <remarks>
        /// This is where the exchanged amount of Items will be <see cref = "ExchangeComponent.RemoveItem">removed</see>
        /// and the exchange will be billed for the previously calculated price.
        /// </remarks>
        /// <param name="item">The Item to sell.</param>
        /// <param name="price">The price the Item is sold for.</param>
        /// <param name="amount">The amount of Items being sold.</param>
        public void RegisterSale(Item item, float price, int amount = 1)
        {
            this.Log($"Exchange registered sale of {amount} {item} {price * amount} ({price})");

            RemoveItem(item, amount);

            ItemSold?.Invoke(this, new ItemSoldEventArgs(item, price, amount));
        }

        /// <summary>
        /// Adds a given amount of a given <see cref = "Item">Item</see> to the <see cref = "ExchangeComponent.GetTargetInventory">target inventory</see>
        /// associated to the given Item for this <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// </summary>
        /// <param name="item">The Item to add.</param>
        /// <param name="amount">The amount of Items to add.</param>
        public void AddItem(Item item, int amount = 1)
        {
            this.Log($"Exchange added {amount} {item} to inventory");

            var targetInventory = GetTargetInventory(item);
            targetInventory.Add(item, amount);
        }

        /// <summary>
        /// Removes a given amount of a given <see cref = "Item">Item</see> from the <see cref = "ExchangeComponent.GetTargetInventory">target inventory</see>
        /// associated to the given Item for this <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// </summary>
        /// <param name="item">The Item to remove.</param>
        /// <param name="amount">The amount of Items to remove.</param>
        public void RemoveItem(Item item, int amount = 1)
        {
            this.Log($"Exchange removed {amount} {item} to inventory");

            var targetInventory = GetTargetInventory(item);
            targetInventory.Remove(item, amount);
        }
    }
}