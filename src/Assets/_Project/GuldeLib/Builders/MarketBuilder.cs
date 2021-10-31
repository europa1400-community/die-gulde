using System.Collections;
using System.Collections.Generic;
using GuldeLib.Company.Employees;
using GuldeLib.Economy;
using GuldeLib.Maps;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Builders
{
    public class MarketBuilder : Builder
    {
        [LoadAsset("prefab_market")]
        GameObject MarketPrefab { get; set; }

        public GameObject MarketObject { get; set; }
        public Dictionary<string, Exchange> Exchanges { get; set; } = new Dictionary<string, Exchange>();
        public Vector3Int EntryCell { get; set; }

        public MarketBuilder WithEntryCell(Vector3Int cellPosition)
        {
            EntryCell = cellPosition;

            return this;
        }

        public MarketBuilder WithEntryCell(int x, int y)
        {
            EntryCell = new Vector3Int(x, y, 0);

            return this;
        }

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
                exchangeComponent.IsAccepting = exchange.IsAccepting;
            }
        }
    }
}