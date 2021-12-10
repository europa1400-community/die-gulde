using GuldeLib.Companies;
using GuldeLib.Economy;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class WealthFactory : Factory<Wealth, WealthComponent>
    {
        public WealthFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override WealthComponent Create(Wealth wealth)
        {
            if (wealth.Exchange.Value)
            {
                var exchangeFactory = new ExchangeFactory(GameObject);
                exchangeFactory.Create(wealth.Exchange.Value);
            }

            Component.Money = wealth.Money;

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