using GuldeLib.Companies;
using GuldeLib.Companies.Carts;
using GuldeLib.Companies.Employees;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class CompanyFactory : Factory<Company>
    {
        public CompanyFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Company company)
        {
            var locationFactory = new LocationFactory(GameObject);
            locationFactory.Create(company.Location);

            if (company.Production)
            {
                var productionFactory = new ProductionFactory(GameObject);
                productionFactory.Create(company.Production);
            }

            if (company.Master)
            {
                var masterFactory = new MasterFactory(GameObject);
                masterFactory.Create(company.Master);
            }

            var companyComponent = GameObject.AddComponent<CompanyComponent>();

            companyComponent.HiringCost = company.HiringCost;
            companyComponent.CartCost = company.CartCost;
            companyComponent.WagePerHour = company.WagePerHour;

            foreach (var employee in company.Employees)
            {
                var employeeFactory = new EmployeeFactory();
                var employeeObject = employeeFactory.Create(employee);

                var employeeComponent = employeeObject.GetComponent<EmployeeComponent>();
                employeeComponent.SetCompany(companyComponent);
            }

            foreach (var cart in company.Carts)
            {
                var cartFactory = new CartFactory();
                var cartObject = cartFactory.Create(cart);

                var cartComponent = cartObject.GetComponent<CartComponent>();
                cartComponent.SetCompany(companyComponent);
            }

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}