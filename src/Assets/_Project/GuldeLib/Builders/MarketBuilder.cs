using System.Collections;
using System.Collections.Generic;
using GuldeLib.Economy;
using GuldeLib.Maps;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Builders
{
    /// <summary>
    /// Provides functionality to build a market.
    /// </summary>
    public class MarketBuilder : Builder
    {
        /// <summary>
        /// Gets the built market's GameObject.
        /// </summary>
        public GameObject MarketObject { get; private set; }

        Market Market { get; }

        /// <summary>
        /// Gets or sets the prefab used to create the market.
        /// </summary>
        [LoadAsset("prefab_market")]
        GameObject MarketPrefab { get; set; }

        /// <summary>
        /// Gets the Dictionary mapping exchange names to exchanges of the built market.
        /// </summary>
        List<Exchange> Exchanges { get; } = new Dictionary<string, Exchange>();

        public MarketBuilder(Market market = null)
        {
            Market = market;
            Market ??= ScriptableObject.CreateInstance<Market>();
        }

        /// <summary>
        /// Sets the entry cell position of the built market to the given value.
        /// </summary>
        public MarketBuilder WithEntryCell(Vector3Int cellPosition)
        {
            EntryCell = cellPosition;
            return this;
        }

        /// <summary>
        /// Sets the entry cell position of the built market.
        /// </summary>
        /// <param name="x">The x coordinate in cells.</param>
        /// <param name="y">The y coordinate in cells.</param>
        public MarketBuilder WithEntryCell(int x, int y)
        {
            EntryCell = new Vector3Int(x, y, 0);
            return this;
        }

        /// <summary>
        /// Requests the market to be built with the given exchange.
        /// </summary>
        public MarketBuilder WithExchange(string name, Exchange exchange)
        {
            if (Exchanges.ContainsKey(name))
            {
                this.Log($"Market already has an Exchange with name {name}.");
                Exchanges[name] = exchange;
            }
            else Exchanges.Add(name, exchange);

            return this;
        }

        /// <inheritdoc cref="WithExchange(string,GuldeLib.Economy.Exchange)"/>
        public MarketBuilder WithExchange(string name, ExchangeBuilder exchangeBuilder)
        {
            var exchange = exchangeBuilder.Build();

            if (Exchanges.ContainsKey(name))
            {
                this.Log($"Market already has an Exchange with name {name}.", LogType.Warning);
                Exchanges[name] = exchange;
            }
            else Exchanges.Add(name, exchange);

            return this;
        }

        /// <inheritdoc cref="Builder.Build"/>
        public override IEnumerator Build()
        {
            yield return base.Build();

            MarketObject = Object.Instantiate(MarketPrefab);

            var location = MarketObject.GetComponent<LocationComponent>();
            location.EntryCell = EntryCell;

            foreach (var pair in Exchanges)
            {
                var name = pair.Key;
                var exchange = pair.Value;

                var child = new GameObject(name);
                child.transform.SetParent(MarketObject.transform);

                var exchangeComponent = child.AddComponent<ExchangeComponent>();
                exchangeComponent.Inventory.Items = exchange.StartItems;
                exchangeComponent.Inventory.Slots = exchange.Slots;
                exchangeComponent.IsPurchasing = exchange.IsPurchasing;
            }
        }
    }
}