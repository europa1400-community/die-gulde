using GuldeLib.Companies;
using GuldeLib.Economy;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class WealthFactory : Factory<Wealth>
    {
        public WealthFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Wealth wealth)
        {
            if (wealth.Exchange.Value)
            {
                var exchangeFactory = new ExchangeFactory(GameObject);
                exchangeFactory.Create(wealth.Exchange.Value);
            }

            var wealthComponent = GameObject.AddComponent<WealthComponent>();

            wealthComponent.Money = wealth.Money;

            foreach (var company in wealth.Companies)
            {
                var companyFactory = new CompanyFactory();
                var companyObject = companyFactory.Create(company.Value);

                var companyComponent = companyObject.GetComponent<CompanyComponent>();
                wealthComponent.RegisterCompany(companyComponent);
            }

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}