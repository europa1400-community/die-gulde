using System.Collections.Generic;
using System.Linq;
using GuldeLib.Company;
using GuldeLib.Economy;
using GuldeLib.Entities;
using GuldeLib.Entities.Pathfinding;
using GuldeLib.Inventory;
using GuldeLib.Production;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldeLib.Vehicles
{
    [RequireComponent(typeof(ExchangeComponent))]
    [RequireComponent(typeof(TravelComponent))]
    [DisallowMultipleComponent]
    public class CartAgentComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public CartState State { get; private set; } = CartState.Idle;

        [ShowInInspector]
        [BoxGroup("Info")]
        [TableList]
        public Queue<ItemOrder> PurchaseOrders { get; } = new Queue<ItemOrder>();

        [ShowInInspector]
        [BoxGroup("Info")]
        [TableList]
        public Queue<ItemOrder> SaleOrders { get; } = new Queue<ItemOrder>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        EntityComponent Entity { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        ExchangeComponent Exchange { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        TravelComponent Travel { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        public CartComponent Cart { get; private set; }

        public bool HasPurchaseOrders => PurchaseOrders.Count > 0;
        public bool HasSaleOrders => SaleOrders.Count > 0;

        int BuyableAmount(ItemOrder order) =>
            Mathf.Min(GetMarketSupply(order.Item), order.Amount);
        ItemOrder FulfillablePurchaseOrder =>
            PurchaseOrders.ToList().Find(e => BuyableAmount(e) > 0);

        int CollectableAmount(ItemOrder order) =>
            Mathf.Min(Cart.Company.Exchange.GetTargetInventory(order.Item).GetSupply(order.Item), order.Amount);
        ItemOrder FulfillableSaleOrder =>
            SaleOrders.ToList().Find(e => CollectableAmount(e) > 0);

        public enum CartState { Idle, Market, Company, }

        void Awake()
        {
            Entity = GetComponent<EntityComponent>();
            Exchange = GetComponent<ExchangeComponent>();
            Travel = GetComponent<TravelComponent>();
            Cart = GetComponent<CartComponent>();
        }

        public void AddPurchaseOrder(ItemOrder order)
        {
            var isFirstOrder = !HasPurchaseOrders && !HasSaleOrders;

            this.Log($"CartAgent got {(isFirstOrder ? "first " : "")}purchase order for {order.Amount} {order.Item}");

            PurchaseOrders.Enqueue(order);

            if (isFirstOrder && State == CartState.Idle && Entity.Location == Cart.Company.Location)
            {
                this.Log($"CartAgent will fulfill the placed purchase order");
                ChangeState(CartState.Market);
            }
        }

        public void AddPurchaseOrders(Queue<ItemOrder> orders)
        {
            var isFirstOrder = !HasPurchaseOrders && !HasSaleOrders;
            this.Log($"CartAgent got {(isFirstOrder ? "first " : "")}purchase orders");

            while (orders.Count > 0)
            {
                var order = orders.Dequeue();
                PurchaseOrders.Enqueue(order);
            }

            if (isFirstOrder && State == CartState.Idle && Entity.Location == Cart.Company.Location)
            {
                this.Log($"CartAgent will fulfill the placed purchase orders");
                ChangeState(CartState.Market);
            }
        }

        public void AddSaleOrder(ItemOrder order)
        {
            var isFirstOrder = !HasPurchaseOrders && !HasSaleOrders;

            this.Log($"CartAgent got {(isFirstOrder ? "first " : "")}sale order for {order.Amount} {order.Item}");

            var targetExchange = Locator.Market.GetExchange(order.Item);

            if (!targetExchange)
            {
                this.Log($"Cart {Cart.name} cannot fullfill order {order}: Market {Locator.Market} does not have a matching exchange", LogType.Error);
                return;
            }

            SaleOrders.Enqueue(order);

            if (isFirstOrder && State == CartState.Idle && Entity.Location == Cart.Company.Location)
            {
                this.Log($"CartAgent will fulfill the placed sale order");
                ChangeState(CartState.Market);
            }
        }

        public void AddSellOrders(Queue<ItemOrder> orders)
        {
            var isFirstOrder = !HasPurchaseOrders && !HasSaleOrders;
            this.Log($"CartAgent got {(isFirstOrder ? "first " : "")}sale orders");

            foreach (var order in orders)
            {
                var targetExchange = Locator.Market.GetExchange(order.Item);

                if (!targetExchange)
                {
                    this.Log(
                        $"Cart {Cart.name} cannot fullfill order {order}: Market {Locator.Market} does not have a matching exchange",
                        LogType.Error);
                    return;
                }
            }

            while (orders.Count > 0)
            {
                var order = orders.Dequeue();
                SaleOrders.Enqueue(order);
            }

            if (isFirstOrder && State == CartState.Idle && Entity.Location == Cart.Company.Location)
            {
                this.Log($"CartAgent will fulfill the placed sale orders");
                ChangeState(CartState.Market);
            }
        }

        void ChangeState(CartState state)
        {
            this.Log($"CartAgent changing state to {state}");

            if (state == CartState.Market)
            {
                State = CartState.Market;
                Cart.Company.Location.EntityRegistry.Registered -= OnCompanyReached;
                Locator.Market.Location.EntityRegistry.Registered += OnMarketReached;

                Travel.TravelTo(Locator.Market.Location);
            }
            else if (state == CartState.Company)
            {
                State = CartState.Company;
                Cart.Company.Location.EntityRegistry.Registered += OnCompanyReached;
                Locator.Market.Location.EntityRegistry.Registered -= OnMarketReached;

                Travel.TravelTo(Cart.Company.Location);
            }
            else if (state == CartState.Idle)
            {
                State = CartState.Idle;
            }
        }

        int GetMarketSupply(Item item)
        {
            var exchange = Locator.Market.GetExchange(item);

            if (!exchange) this.Log($"Couldn't find an exchange for {item.Name}");
            if (!exchange) return 0;

            this.Log($"Market supply for {item.Name} is {exchange.Inventory.GetSupply(item)}");
            return exchange.Inventory.GetSupply(item);
        }

        void FulfillPurchaseOrders()
        {
            this.Log("Buying items from market.");

            //TODO auch Slots berücksichtigen, wo ein Item aus den Orders drin ist
            for (var i = 0; i < Exchange.Inventory.FreeSlots; i++)
            {
                var order = FulfillablePurchaseOrder;

                if (order == null)
                {
                    this.Log("No more orders are fulfillable.");
                    break;
                }

                this.Log($"Fulfilling order for {order.Amount} {order.Item.Name}");

                var marketExchange = Locator.Market.GetExchange(order.Item);
                var amount = BuyableAmount(order);

                Exchange.BuyItem(order.Item, marketExchange, amount);

                order.Amount -= amount;
                if (order.Amount <= 0)
                {
                    this.Log($"Completely fulfilled order for {order.Item.Name}");
                    PurchaseOrders.Dequeue();
                }
            }
        }

        void FulfillSaleOrders()
        {
            this.Log("Selling items to market.");

            while (HasSaleOrders)
            {
                var order = SaleOrders.FirstOrDefault(e => Cart.Inventory.HasProductInStock(e.Item));
                if (order == null) return;

                var targetExchange = Locator.Market.GetExchange(order.Item);
                var amount = Cart.Inventory.GetSupply(order.Item);

                this.Log($"Fulfilling order for {amount} {order.Item.Name}");

                Exchange.SellItem(order.Item, targetExchange, order.Amount);

                if (order.Amount <= 0)
                {
                    this.Log($"Completely fulfilled order for {order.Item.Name}");
                    PurchaseOrders.Dequeue();
                }
            }
        }

        void ResupplyCompany()
        {
            this.Log($"CartAgent resupplying the company");
            var itemsToTransfer = new Dictionary<Item, int>(Exchange.Inventory.Items);

            foreach (var pair in itemsToTransfer)
            {
                var item = pair.Key;
                var supply = pair.Value;

                if (!Exchange.CanSellTo(item, Cart.Company.Exchange))
                {
                    this.Log($"CartAgent can't resupply with {item}: Can't transfer item");
                    continue;
                }

                this.Log($"CartAgent resupplying company with {supply} {item}");
                Exchange.SellItem(item, Cart.Company.Exchange, supply);
            }
        }

        void CollectSaleItems()
        {
            if (!HasSaleOrders) return;

            this.Log($"CartAgent collecting items from company");

            for (var i = 0; i < Exchange.Inventory.FreeSlots; i++)
            {
                var order = FulfillableSaleOrder;

                if (order == null)
                {
                    this.Log("No more orders are fulfillable.");
                    break;
                }

                this.Log($"Fulfilling order for {order.Amount} {order.Item.Name}");

                var targetInventory = Cart.Company.Exchange.GetTargetInventory(order.Item);
                var amount = Mathf.Min(order.Amount, targetInventory.GetSupply(order.Item));

                Exchange.BuyItem(order.Item, Cart.Company.Exchange, amount);

                order.Amount -= amount;
            }
        }

        void OnMarketReached(object sender, EntityEventArgs entityEventArgs)
        {
            FulfillSaleOrders();
            FulfillPurchaseOrders();

            var wasSuccessful = !Exchange.Inventory.IsEmpty;

            if (wasSuccessful) this.Log($"Succesfully fulfilled orders. Returning to company {Cart.Company.name}.");
            else this.Log("No items in stock. Waiting for market resupply.");

            if (wasSuccessful) ChangeState(CartState.Company);
            else
            {
                foreach (var exchange in Locator.Market.Location.Exchanges)
                {
                    exchange.Inventory.Added += OnMarketResupplied;
                }
            }
        }

        void OnMarketResupplied(object sender, ItemEventArgs e)
        {
            if (PurchaseOrders.All(o => o.Item != e.Item)) return;

            FulfillPurchaseOrders();

            var wasSuccessful = !Exchange.Inventory.IsEmpty;

            if (wasSuccessful)
            {
                foreach (var exchange in Locator.Market.Location.Exchanges)
                {
                    exchange.Inventory.Added -= OnMarketResupplied;
                }

                ChangeState(CartState.Company);
            }
        }

        void OnCompanyReached(object sender, EntityEventArgs entityEventArgs)
        {
            this.Log($"{name} returned to {Cart.Company.name} and will resupply");

            ResupplyCompany();

            if (Exchange.Inventory.IsFull)
            {
                Cart.Company.Production.Registry.RecipeFinished += OnRecipeFinished;
                return;
            }

            CollectSaleItems();

            if (HasPurchaseOrders || HasSaleOrders) ChangeState(CartState.Market);
            else ChangeState(CartState.Idle);
        }

        void OnRecipeFinished(object sender, ProductionEventArgs e)
        {
            ResupplyCompany();

            if (!Exchange.Inventory.IsFull)
            {
                this.Log($"CartAgent resupplied company successfully");
                Cart.Company.Production.Registry.RecipeFinished -= OnRecipeFinished;

                CollectSaleItems();

                if (HasPurchaseOrders || HasSaleOrders) ChangeState(CartState.Market);
                else ChangeState(CartState.Idle);
            }
        }
    }
}