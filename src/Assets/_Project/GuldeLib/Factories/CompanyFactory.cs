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
        public CompanyFactory(Company company, GameObject gameObject = null, GameObject parentObject = null) : base(company, gameObject, parentObject)
        {
        }

        public override CompanyComponent Create() => null;

        public CompanyComponent Create(MapComponent mapComponent)
        {
            Component.HiringCost = TypeObject.HiringCost;
            Component.CartCost = TypeObject.CartCost;
            Component.WagePerHour = TypeObject.WagePerHour;
            Component.EmployeeTemplates = TypeObject.Employees;
            Component.CartTemplates = TypeObject.Carts;

            var locationFactory = new LocationFactory(TypeObject.Location.Value, GameObject);
            var locationComponent = locationFactory.Create();

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

            if (TypeObject.Production.Value)
            {
                var productionFactory = new ProductionFactory(TypeObject.Production.Value, GameObject);
                productionFactory.Create();
            }

            Component.HireEmployee();
            Component.HireCart();

            if (TypeObject.Master.Value)
            {
                var masterFactory = new MasterFactory(TypeObject.Master.Value, GameObject);
                masterFactory.Create();
            }

            return Component;
        }
    }
}