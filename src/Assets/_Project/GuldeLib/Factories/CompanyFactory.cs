using GuldeLib.Companies;
using GuldeLib.Companies.Carts;
using GuldeLib.Companies.Employees;
using GuldeLib.Maps;
using GuldeLib.Maps.Buildings;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class CompanyFactory : Factory<Company, CompanyComponent>
    {
        public CompanyFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override CompanyComponent Create(Company company) => null;

        public CompanyComponent Create(Company company, MapComponent mapComponent)
        {
            Component.HiringCost = company.HiringCost;
            Component.CartCost = company.CartCost;
            Component.WagePerHour = company.WagePerHour;
            Component.EmployeeTemplates = company.Employees;
            Component.CartTemplates = company.Carts;

            var locationFactory = new LocationFactory(GameObject);
            var locationComponent = locationFactory.Create(company.Location.Value);

            var buildingComponent = GameObject.GetComponent<BuildingComponent>();

            var companyEntryCell = mapComponent.MapLayout.PlaceBuilding(buildingComponent.Building);

            if (!companyEntryCell.HasValue)
            {
                this.Log($"Could not create company: No build space available.", LogType.Warning);
                Object.Destroy(GameObject);
                return null;
            }

            locationComponent.EntityArrived += Component.OnEntityArrived;
            locationComponent.EntityLeft += Component.OnEntityLeft;
            if (Locator.Time) Locator.Time.WorkingHourTicked += Component.OnWorkingHourTicked;

            mapComponent.Register(locationComponent);
            locationComponent.EntryCell = companyEntryCell.Value;

            if (company.Production.Value)
            {
                var productionFactory = new ProductionFactory(GameObject);
                productionFactory.Create(company.Production.Value);
            }

            Component.HireEmployee();
            Component.HireCart();

            if (company.Master.Value)
            {
                var masterFactory = new MasterFactory(GameObject);
                masterFactory.Create(company.Master.Value);
            }

            return Component;
        }
    }
}