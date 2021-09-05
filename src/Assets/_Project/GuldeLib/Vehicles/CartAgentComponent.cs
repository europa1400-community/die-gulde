using System.Collections.Generic;
using System.Linq;
using GuldeLib.Company;
using GuldeLib.Economy;
using GuldeLib.Entities;
using GuldeLib.Pathfinding;
using GuldeLib.Production;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Vehicles
{
    [RequireComponent(typeof(ExchangeComponent))]
    [RequireComponent(typeof(TravelComponent))]
    [RequireComponent(typeof(PathfindingComponent))]
    [DisallowMultipleComponent]
    public class CartAgentComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public CompanyComponent Company { get; set; }

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
        PathfindingComponent Pathfinding { get; set; }

        [OdinSerialize]
        [TableList]
        public Queue<ItemOrder> Orders { get; private set; } = new Queue<ItemOrder>();

        bool HasOrders => Orders.Count > 0;

        public enum CartState { Buying, Resupply, }

        [OdinSerialize]
        CartState State { get; set; } = CartState.Resupply;

        void Awake()
        {
            Entity = GetComponent<EntityComponent>();
            Exchange = GetComponent<ExchangeComponent>();
            Travel = GetComponent<TravelComponent>();
            Pathfinding = GetComponent<PathfindingComponent>();
        }

        public void AddOrder(ItemOrder order)
        {
            var isFirstOrder = !HasOrders;

            Orders.Enqueue(order);

            if (isFirstOrder && State == CartState.Resupply && Entity.Location == Company.Location)
                ChangeState(CartState.Buying);
        }

        void ChangeState(CartState state)
        {
            if (state == CartState.Buying)
            {
                State = CartState.Buying;
                Company.Location.EntityRegistry.Registered -= OnCompanyReached;
                Locator.Market.Location.EntityRegistry.Registered += OnMarketReached;

                Travel.TravelTo(Locator.Market.Location);
            }
            else
            {
                State = CartState.Resupply;
                Company.Location.EntityRegistry.Registered += OnCompanyReached;
                Locator.Market.Location.EntityRegistry.Registered -= OnMarketReached;

                Travel.TravelTo(Company.Location);
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
                    this.Log($"Completely fulfilled order for {order.Amount} {order.Item.Name}");
                    Orders.Dequeue();
                }
            }
        }

        void ResupplyCompany()
        {
            foreach (var item in Exchange.Inventory.Items)
            {
                if (!Exchange.CanSellTo(item, Company.Exchange)) continue;

                Exchange.SellItem(item, Company.Exchange, item.Supply);
            }
        }

        void OnMarketReached(object sender, EntityEventArgs entityEventArgs)
        {
            FulfillOrders();

            var wasSuccessful = !Exchange.Inventory.IsEmpty;

            if (wasSuccessful) this.Log($"Succesfully fulfilled orders. Returning to company {Company.name}.");
            else this.Log("No items in stock. Waiting for market resupply.");

            if (wasSuccessful) ChangeState(CartState.Resupply);
            else
            {
                foreach (var exchange in Locator.Market.Location.Exchanges)
                {
                    exchange.ItemBought += OnMarketResupplied;
                }
            }
        }

        void OnMarketResupplied(object sender, ExchangeEventArgs e)
        {
            if (Orders.All(o => o.Item != e.Item)) return;

            FulfillOrders();

            var wasSuccessful = !Exchange.Inventory.IsEmpty;

            if (wasSuccessful)
            {
                foreach (var exchange in Locator.Market.Location.Exchanges)
                {
                    exchange.ItemBought -= OnMarketResupplied;
                }

                ChangeState(CartState.Resupply);
            }
        }

        void OnCompanyReached(object sender, EntityEventArgs entityEventArgs)
        {
            this.Log($"{name} returned to {Company.name} and will resupply");

            ResupplyCompany();

            if (Exchange.Inventory.IsFull)
            {
                Company.Production.Registry.RecipeFinished += OnRecipeFinished;
            }
            else if (HasOrders) ChangeState(CartState.Buying);
        }

        void OnRecipeFinished(object sender, ProductionEventArgs e)
        {
            ResupplyCompany();

            if (!Exchange.Inventory.IsFull)
            {
                Company.Production.Registry.RecipeFinished -= OnRecipeFinished;

                if (HasOrders) ChangeState(CartState.Buying);
            }
        }
    }
}