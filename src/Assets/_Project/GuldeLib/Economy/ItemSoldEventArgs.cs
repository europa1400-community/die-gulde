using System;

namespace GuldeLib.Economy
{
    /// <summary>
    /// Contains arguments for the <see cref = "ExchangeComponent.ItemSold"/> event.
    /// </summary>
    public class ItemSoldEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref = "Item">Item</see> that was sold.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets the price the <see cref = "Item">Item</see> was sold for.
        /// </summary>
        public float Price { get; }

        /// <summary>
        /// Gets the amount of <see cref = "Item">Items</see> that were sold.
        /// </summary>
        public int Amount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref = "ItemSoldEventArgs">ItemSoldEventArgs</see> class.
        /// </summary>
        /// <param name="item"><see cref = "ItemSoldEventArgs.Item">Sold Item.</see></param>
        /// <param name="price"><see cref = "ItemSoldEventArgs.Price">Sell price.</see></param>
        /// <param name="amount"><see cref = "ItemSoldEventArgs.Amount">Sold amount.</see></param>
        public ItemSoldEventArgs(Item item, float price, int amount = 1)
        {
            Item = item;
            Price = price;
            Amount = amount;
        }
    }
}