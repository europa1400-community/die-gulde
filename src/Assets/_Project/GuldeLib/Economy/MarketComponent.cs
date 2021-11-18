using System.Collections.Generic;
using GuldeLib.Maps;
using MonoExtensions.Runtime;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Economy
{
    /// <summary>
    /// Provides functionality for multi-exchange markets.
    /// </summary>
    [RequireComponent(typeof(LocationComponent))]
    public class MarketComponent : SerializedMonoBehaviour
    {
        /// <summary>
        /// Gets the <see cref = "LocationComponent">LocationComponent</see> associated to this market.
        /// </summary>
        [ShowInInspector]
        public LocationComponent Location => this.GetCachedComponent<LocationComponent>();

        /// <summary>
        /// Gets the dictionary mapping <see cref = "Item">Items</see> to the <see cref = "ExchangeComponent">ExchangeComponent</see> responsible for trading the Item.
        /// </summary>
        Dictionary<Item, ExchangeComponent> ItemToExchange { get; } = new Dictionary<Item, ExchangeComponent>();

        /// <summary>
        /// Gets the <see cref = "ExchangeComponent">ExchangeComponent</see> responsible for trading a given <see cref = "Item">Item</see>.
        /// </summary>
        /// <param name="item">The item to check.</param>
        public ExchangeComponent GetExchange(Item item)
        {
            if (ItemToExchange.ContainsKey(item)) return ItemToExchange[item];

            var exchange = Location.Exchanges.Find(e => e.Inventory.IsRegistered(item));
            if (!exchange) return null;

            ItemToExchange.Add(item, exchange);

            return exchange;
        }

        /// <summary>
        /// Gets the price a given <see cref = "Item">Item</see> is traded for at the <see cref = "GetExchange">responsible</see> <see cref = "ExchangeComponent">ExchangeComponent</see>.
        /// </summary>
        /// <param name="item">The Item to check.</param>
        public float GetPrice(Item item)
        {
            var exchange = GetExchange(item);
            if (!exchange)
            {
                this.Log($"Market cannot get price for {item}: No exchange found");
                return item.MeanPrice;
            }

            return exchange.GetPrice(item);
        }

        /// <remarks>
        /// Initializes references to <see cref = "MarketComponent.Location">Location</see>
        /// and assigns this <see cref = "MarketComponent">MarketComponent</see> as the globally accessible market service.
        /// </remarks>
        void Awake()
        {
            this.Log("Market initializing");
            Locator.Market = this;
        }
    }
}