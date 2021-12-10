using GuldeLib.Companies;
using GuldeLib.Inventories;
using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ProductionFactory : Factory<Production, ProductionComponent>
    {
        public ProductionFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override ProductionComponent Create(Production production)
        {
            var assignmentFactory = new AssignmentFactory(GameObject);
            var assignmentComponent = assignmentFactory.Create(production.Assignment.Value);

            var exchangeFactory = new ExchangeFactory(GameObject);
            exchangeFactory.Create(production.Exchange.Value);

            var productionRegistryFactory = new ProductionRegistryFactory(GameObject);
            var productionRegistryComponent = productionRegistryFactory.Create(production.ProductionRegistry.Value);

            var companyComponent = GameObject.GetComponent<CompanyComponent>();
            var resourceInventoryComponent = GameObject.GetComponent<InventoryComponent>();

            assignmentComponent.Assigned += Component.OnEmployeeAssigned;
            assignmentComponent.Unassigned += Component.OnEmployeeUnassigned;
            companyComponent.EmployeeArrived += Component.OnEmployeeArrived;
            productionRegistryComponent.RecipeFinished += Component.OnRecipeFinished;
            resourceInventoryComponent.Added += Component.OnItemAdded;
            if (Locator.Time) Locator.Time.Evening += Component.OnEvening;

            return Component;
        }
    }
}