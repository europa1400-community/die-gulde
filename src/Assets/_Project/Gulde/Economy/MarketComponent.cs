using System.Collections.Generic;
using Gulde.Maps;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gulde.Economy
{
    [RequireComponent(typeof(LocationComponent))]
    public class MarketComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [ReadOnly]
        LocationComponent Location { get; set; }

        Dictionary<Item, ExchangeComponent> ItemToExchange { get; } = new Dictionary<Item, ExchangeComponent>();

        public ExchangeComponent GetExchange(Item item)
        {
            if (ItemToExchange.ContainsKey(item)) return ItemToExchange[item];

            var exchange = Location.Exchanges.Find(e => e.Inventory.IsRegistered(item));
            if (!exchange) return null;

            ItemToExchange.Add(item, exchange);

            return exchange;
        }

        public float GetPrice(Item item)
        {
            var exchange = GetExchange(item);
            if (!exchange) return item.MeanPrice;

            return exchange.GetPrice(item);
        }

        void Awake()
        {
            Location = GetComponent<LocationComponent>();
            Locator.Market = this;
        }
    }
}