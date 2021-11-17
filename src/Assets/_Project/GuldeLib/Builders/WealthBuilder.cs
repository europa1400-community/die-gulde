using System.Collections.Generic;
using GuldeLib.Companies;
using GuldeLib.Economy;

namespace GuldeLib.Builders
{
    public class WealthBuilder : Builder<Wealth>
    {
        public WealthBuilder WithMoney(float money)
        {
            Object.Money = money;
            return this;
        }

        public WealthBuilder WithCompanies(List<Company> companies)
        {
            Object.Companies = companies;
            return this;
        }

        public WealthBuilder WithExchange(Exchange exchange)
        {
            Object.Exchange = exchange;
            return this;
        }
    }
}