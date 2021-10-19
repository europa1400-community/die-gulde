using System;
using System.Collections.Generic;
using GuldeLib.Economy;
using JetBrains.Annotations;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Builders
{
    public class ExchangeBuilder
    {
        Exchange Exchange { get; }

        public ExchangeBuilder()
        {
            Exchange = ScriptableObject.CreateInstance<Exchange>();
        }

        public ExchangeBuilder WithStartItems([NotNull] Dictionary<Item, int> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            Exchange.StartItems = items;

            return this;
        }

        public ExchangeBuilder WithItem(Item item, int supply)
        {
            if (Exchange.StartItems.ContainsKey(item))
            {
                this.Log($"Exchange already has item with name {item}.", LogType.Warning);

                Exchange.StartItems[item] = supply;
            }
            else Exchange.StartItems.Add(item, supply);

            return this;
        }

        public ExchangeBuilder WithSlots(int slots)
        {
            Exchange.Slots = slots;

            return this;
        }

        public ExchangeBuilder WithAccepting(bool isAccepting)
        {
            Exchange.IsAccepting = isAccepting;

            return this;
        }

        public Exchange Build() => Exchange;
    }
}