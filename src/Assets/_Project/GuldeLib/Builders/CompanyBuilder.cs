using System.Collections.Generic;
using GuldeLib.Companies;
using GuldeLib.Companies.Carts;
using GuldeLib.Companies.Employees;
using GuldeLib.Generators;
using GuldeLib.Maps;
using GuldeLib.Producing;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class CompanyBuilder : Builder<Company>
    {
        public CompanyBuilder WithHiringCost(int hiringCost)
        {
            Object.HiringCost = hiringCost;
            return this;
        }

        public CompanyBuilder WithCartCost(int cartCost)
        {
            Object.CartCost = cartCost;
            return this;
        }

        public CompanyBuilder WithWagePerHour(float wagePerHour)
        {
            Object.WagePerHour = wagePerHour;
            return this;
        }

        public CompanyBuilder WithEmployees(List<GeneratableEmployee> employees)
        {
            Object.Employees = employees;
            return this;
        }

        public CompanyBuilder WithCarts(List<GeneratableCart> carts)
        {
            Object.Carts = carts;
            return this;
        }

        public CompanyBuilder WithLocation(GeneratableLocation location)
        {
            Object.Location = location;
            return this;
        }

        public CompanyBuilder WithProduction(GeneratableProduction production)
        {
            Object.Production = production;
            return this;
        }

        public CompanyBuilder WithMaster(GeneratableMaster master)
        {
            Object.Master = master;
            return this;
        }
    }
}