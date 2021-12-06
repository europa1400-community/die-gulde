using System.Collections.Generic;
using GuldeLib.Companies;
using GuldeLib.Economy;
using GuldeLib.Generators;

namespace GuldeLib.Builders
{
    public class WealthBuilder : Builder<Wealth>
    {
        public WealthBuilder WithMoney(float money)
        {
            Object.Money = money;
            return this;
        }

        public WealthBuilder WithCompanies(List<GeneratableCompany> companies)
        {
            Object.Companies = companies;
            return this;
        }

        public WealthBuilder WithExchange(GeneratableExchange exchange)
        {
            Object.Exchange = exchange;
            return this;
        }
    }
}