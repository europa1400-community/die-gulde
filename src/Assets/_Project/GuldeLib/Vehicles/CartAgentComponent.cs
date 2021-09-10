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
        [OdinSerialize]
        [ReadOnly]
        EntityComponent Entity { get; set; }

        [OdinSerialize]
        [ReadOnly]
        ExchangeComponent Exchange { get; set; }

        [OdinSerialize]
        [ReadOnly]
        TravelComponent Travel { get; set; }

        [OdinSerialize]
        [ReadOnly]
        public CartComponent Cart { get; private set; }

        [OdinSerialize]
        [TableList]
        public Queue<ItemOrder> Orders { get; } = new Queue<ItemOrder>();

        public bool HasOrders => Orders.Count > 0;

        public enum CartState { Idle, Buying, Resupply, }

        [OdinSerialize]
        public CartState State { get; private set; } = CartState.Idle;

        void Awake()
        {
            Entity = GetComponent<EntityComponent>();
            Exchange = GetComponent<ExchangeComponent>();
            Travel = GetComponent<TravelComponent>();
            Cart = GetComponent<CartComponent>();
        }

        public void AddOrder(ItemOrder order)
        {
            var isFirstOrder = !HasOrders;

            this.Log($"CartAgent got {(isFirstOrder ? "first " : "")}order for {order.Amount} {order.Item}");

            Orders.Enqueue(order);

            if (isFirstOrder && State == CartState.Idle && Entity.Location == Cart.Company.Location)
            {
                this.Log($"CartAgent will fulfill the placed order");
                ChangeState(CartState.Buying);
            }
        }

        void ChangeState(CartState state)
        {
            this.Log($"CartAgent changing state to {state}");

            if (state == CartState.Buying)
            {
                State = CartState.Buying;
                Cart.Company.Location.EntityRegistry.Registered -= OnCompanyReached;
                Locator.Market.Location.EntityRegistry.Registered += OnMarketReached;

                Travel.TravelTo(Locator.Market.Location);
            }
            else if (state == CartState.Resupply)
            {
                State = CartState.Resupply;
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

        int BuyableAmount(ItemOrder order) => Mathf.Min(GetMarketSupply(order.Item), order.Amount);

        ItemOrder FulfillableOrder => Orders.ToList().Find(e => BuyableAmount(e) > 0);

        void FulfillOrders()
        {
            this.Log("Buying items from market.");

            //TODO auch Slots berücksichtigen, wo ein Item aus den Orders drin ist und noch nicht voll ist
            for (var i = 0; i < Exchange.Inventory.FreeSlots; i++)
            {
                var order = FulfillableOrder;

                if (order == null)
                {
                    this.Log("No order is fulfillable.");
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
                    Orders.Dequeue();
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

        void OnMarketReached(object sender, EntityEventArgs entityEventArgs)
        {
            FulfillOrders();

            var wasSuccessful = !Exchange.Inventory.IsEmpty;

            if (wasSuccessful) this.Log($"Succesfully fulfilled orders. Returning to company {Cart.Company.name}.");
            else this.Log("No items in stock. Waiting for market resupply.");

            if (wasSuccessful) ChangeState(CartState.Resupply);
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
            if (Orders.All(o => o.Item != e.Item)) return;

            FulfillOrders();

            var wasSuccessful = !Exchange.Inventory.IsEmpty;

            if (wasSuccessful)
            {
                foreach (var exchange in Locator.Market.Location.Exchanges)
                {
                    exchange.Inventory.Added -= OnMarketResupplied;
                }

                ChangeState(CartState.Resupply);
            }
        }

        void OnCompanyReached(object sender, EntityEventArgs entityEventArgs)
        {
            this.Log($"{name} returned to {Cart.Company.name} and will resupply");

            ResupplyCompany();

            if (Exchange.Inventory.IsFull)
            {
                Cart.Company.Production.Registry.RecipeFinished += OnRecipeFinished;
            }
            else if (HasOrders) ChangeState(CartState.Buying);
            else ChangeState(CartState.Idle);
        }

        void OnRecipeFinished(object sender, ProductionEventArgs e)
        {
            ResupplyCompany();

            if (!Exchange.Inventory.IsFull)
            {
                this.Log($"CartAgent resupplied company successfully");
                Cart.Company.Production.Registry.RecipeFinished -= OnRecipeFinished;

                if (HasOrders) ChangeState(CartState.Buying);
                else ChangeState(CartState.Idle);
            }
        }
    }
}