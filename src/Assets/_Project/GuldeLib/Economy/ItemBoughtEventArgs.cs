namespace GuldeLib.Economy
{
    /// <summary>
    /// Contains arguments for the <see cref = "ExchangeComponent.ItemBought"/> event.
    /// </summary>
    public class ItemBoughtEventArgs
    {
        /// <summary>
        /// Gets the <see cref = "Item">Item</see> that was bought.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets the price the <see cref = "Item">Item</see> was bought for.
        /// </summary>
        public float Price { get; }

        /// <summary>
        /// Gets the amount of <see cref = "Item">Items</see> that were bought.
        /// </summary>
        public int Amount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref = "ItemBoughtEventArgs">ItemBoughtEventArgs</see> class.
        /// </summary>
        /// <param name="item"><see cref = "ItemBoughtEventArgs.Item">Bought Item.</see></param>
        /// <param name="price"><see cref = "ItemBoughtEventArgs.Price">Purchase price.</see></param>
        /// <param name="amount"><see cref = "ItemBoughtEventArgs.Amount">Purchased amount.</see></param>
        public ItemBoughtEventArgs(Item item, float price, int amount = 1)
        {
            Item = item;
            Price = price;
            Amount = amount;
        }
    }
}