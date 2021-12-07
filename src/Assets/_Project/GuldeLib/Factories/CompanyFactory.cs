using GuldeLib.Companies;
using GuldeLib.Companies.Carts;
using GuldeLib.Companies.Employees;
using GuldeLib.TypeObjects;
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
            locationFactory.Create(company.Location.Value);

            if (company.Production.Value)
            {
                var productionFactory = new ProductionFactory(GameObject);
                productionFactory.Create(company.Production.Value);
            }

            if (company.Master.Value)
            {
                var masterFactory = new MasterFactory(GameObject);
                masterFactory.Create(company.Master.Value);
            }

            var companyComponent = GameObject.AddComponent<CompanyComponent>();

            companyComponent.HiringCost = company.HiringCost;
            companyComponent.CartCost = company.CartCost;
            companyComponent.WagePerHour = company.WagePerHour;

            foreach (var employee in company.Employees)
            {
                var employeeFactory = new EmployeeFactory(parentObject: GameObject);
                var employeeObject = employeeFactory.Create(employee.Value);

                var employeeComponent = employeeObject.GetComponent<EmployeeComponent>();
                employeeComponent.SetCompany(companyComponent);
            }

            foreach (var cart in company.Carts)
            {
                var cartFactory = new CartFactory(parentObject: GameObject);
                var cartObject = cartFactory.Create(cart.Value);

                var cartComponent = cartObject.GetComponent<CartComponent>();
                cartComponent.SetCompany(companyComponent);
            }

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}