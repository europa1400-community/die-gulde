using GuldeLib.Companies.Carts;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class CartFactory : Factory<Cart, CartComponent>
    {
        public CartFactory(GameObject parentObject) : base(null, parentObject)
        {
        }

        public override CartComponent Create(Cart cart)
        {
            Component.CartType = cart.CartType;

            var exchangeFactory = new ExchangeFactory(GameObject);
            exchangeFactory.Create(cart.Exchange.Value);

            var travelFactory = new TravelFactory(GameObject, ParentObject);
            var travelComponent = travelFactory.Create(cart.Travel.Value);

            travelComponent.DestinationReached += Component.OnDestinationReached;

            return Component;
        }
    }
}