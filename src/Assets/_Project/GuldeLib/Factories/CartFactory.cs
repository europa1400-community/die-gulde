using GuldeLib.Companies.Carts;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class CartFactory : Factory<Cart>
    {
        public CartFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Cart cart)
        {
            var exchangeFactory = new ExchangeFactory(GameObject);
            exchangeFactory.Create(cart.Exchange.Value);

            var travelFactory = new TravelFactory(GameObject);
            travelFactory.Create(cart.Travel.Value);

            var cartComponent = GameObject.AddComponent<CartComponent>();

            cartComponent.CartType = cart.CartType;

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}