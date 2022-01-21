using GuldeLib.Companies;
using GuldeLib.Inventories;
using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ProductionFactory : Factory<Production, ProductionComponent>
    {
        public ProductionFactory(Production production, GameObject gameObject = null, GameObject parentObject = null) : base(production, gameObject, parentObject)
        {
        }

        public override ProductionComponent Create()
        {
            var assignmentFactory = new AssignmentFactory(TypeObject.Assignment.Value, GameObject);
            var assignmentComponent = assignmentFactory.Create();

            var exchangeFactory = new ExchangeFactory(TypeObject.Exchange.Value, GameObject);
            exchangeFactory.Create();

            var productionRegistryFactory = new ProductionRegistryFactory(TypeObject.ProductionRegistry.Value, GameObject);
            var productionRegistryComponent = productionRegistryFactory.Create();

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