using System;
using System.Collections.Generic;
using GuldeLib.Economy;
using JetBrains.Annotations;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Builders
{
    /// <summary>
    /// Provides functionality to build an <see cref = "Economy.Exchange">Exchange</see>.
    /// </summary>
    public class ExchangeBuilder
    {
        /// <summary>
        /// Gets the built <see cref = "Economy.Exchange">Exchange</see>.
        /// </summary>
        Exchange Exchange { get; }

        /// <summary>
        /// Initializes an new instance of the <see cref = "ExchangeBuilder">ExchangeBuilder</see> class.
        /// </summary>
        public ExchangeBuilder()
        {
            Exchange = ScriptableObject.CreateInstance<Exchange>();
        }

        /// <summary>
        /// Sets the starting <see cref = "Item">Items</see> of the <see cref = "Economy.Exchange">Exchange</see>.
        /// </summary>
        /// <param name="items">A dictionary mapping <see cref = "Item">Items</see> to their amounts.</param>
        public ExchangeBuilder WithStartItems([NotNull] Dictionary<Item, int> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            Exchange.StartItems = items;

            return this;
        }

        /// <summary>
        /// Adds a given amount of a given <see cref = "Item">Item</see> to the <see cref = "Economy.Exchange">Exchange's</see> <see cref = "Economy.Exchange.StartItems">StartItems</see>.
        /// </summary>
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

        /// <summary>
        /// Sets the amount of slots in the <see cref = "Economy.Exchange">Exchange's</see> <see cref = "Inventory.InventoryComponent">Inventory</see>.
        /// </summary>
        public ExchangeBuilder WithSlots(int slots)
        {
            Exchange.Slots = slots;

            return this;
        }

        /// <summary>
        /// Sets whether the <see cref = "Economy.Exchange">Exchange</see> accepts <see cref = "Item">Items</see> given to it.
        /// </summary>
        public ExchangeBuilder WithAccepting(bool isAccepting)
        {
            Exchange.IsAccepting = isAccepting;

            return this;
        }

        /// <summary>
        /// Builds the defined <see cref = "Economy.Exchange">Exchange</see>.
        /// </summary>
        /// <returns>The built <see cref = "Economy.Exchange">Exchange</see>.</returns>
        public Exchange Build() => Exchange;
    }
}