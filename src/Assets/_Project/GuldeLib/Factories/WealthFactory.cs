using GuldeLib.Companies;
using GuldeLib.Economy;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class WealthFactory : Factory<Wealth, WealthComponent>
    {
        public WealthFactory(Wealth wealth, GameObject gameObject = null, GameObject parentObject = null) : base(wealth, gameObject, parentObject)
        {
        }

        public override WealthComponent Create()
        {
            if (TypeObject.Exchange.Value)
            {
                var exchangeFactory = new ExchangeFactory(TypeObject.Exchange.Value, GameObject);
                exchangeFactory.Create();
            }

            Component.Money = TypeObject.Money;

            var exchangeComponent = GameObject.GetComponent<ExchangeComponent>();

            if (exchangeComponent)
            {
                exchangeComponent.ItemSold += Component.OnItemSold;
                exchangeComponent.ItemBought += Component.OnItemBought;
            }

            if (Locator.Time) Locator.Time.YearTicked += Component.OnYearTicked;

            return Component;
        }
    }
}