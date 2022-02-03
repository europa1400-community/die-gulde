using GuldeLib.Companies.Carts;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class CartFactory : Factory<Cart, CartComponent>
    {
        public CartFactory(Cart cart, GameObject parentObject) : base(cart, null, parentObject)
        {
        }

        public override CartComponent Create()
        {
            Component.Type = TypeObject.CartType;

            var exchangeFactory = new ExchangeFactory(TypeObject.Exchange.Value, GameObject);
            exchangeFactory.Create();

            var travelFactory = new TravelFactory(TypeObject.Travel.Value, GameObject, ParentObject);
            var travelComponent = travelFactory.Create();

            travelComponent.DestinationReached += Component.OnDestinationReached;

            return Component;
        }
    }
}