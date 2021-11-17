using System.Collections.Generic;
using GuldeLib.Companies;
using GuldeLib.Companies.Carts;
using GuldeLib.Companies.Employees;
using GuldeLib.Maps;
using GuldeLib.Producing;

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

        public CompanyBuilder WithEmployees(List<Employee> employees)
        {
            Object.Employees = employees;
            return this;
        }

        public CompanyBuilder WithCarts(List<Cart> carts)
        {
            Object.Carts = carts;
            return this;
        }

        public CompanyBuilder WithLocation(Location location)
        {
            Object.Location = location;
            return this;
        }

        public CompanyBuilder WithProduction(Production production)
        {
            Object.Production = production;
            return this;
        }

        public CompanyBuilder WithMaster(Master master)
        {
            Object.Master = master;
            return this;
        }
    }
}