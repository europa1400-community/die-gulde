using GuldeLib.Companies.Carts;
using GuldeLib.Economy;
using GuldeLib.Entities;

namespace GuldeLib.Builders
{
    public class CartBuilder : Builder<Cart>
    {
        public CartBuilder WithCartType(CartType cartType)
        {
            Object.CartType = cartType;
            return this;
        }

        public CartBuilder WithTravel(Travel travel)
        {
            Object.Travel = travel;
            return this;
        }

        public CartBuilder WithExchange(Exchange exchange)
        {
            Object.Exchange = exchange;
            return this;
        }
    }
}