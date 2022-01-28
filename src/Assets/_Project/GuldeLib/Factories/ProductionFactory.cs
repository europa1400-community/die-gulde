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
            CreateAssignment();

            CreateExchange();

            CreateProductionRegistry();

            SetupCompany();

            SetupResourceInventory();

            SetupTime();

            return Component;
        }

        void CreateAssignment()
        {
            var assignmentFactory = new AssignmentFactory(TypeObject.Assignment.Value, GameObject);
            var assignmentComponent = assignmentFactory.Create();

            assignmentComponent.Assigned += Component.OnEmployeeAssigned;
            assignmentComponent.Unassigned += Component.OnEmployeeUnassigned;
        }

        void CreateExchange()
        {
            var exchangeFactory = new ExchangeFactory(TypeObject.Exchange.Value, GameObject);
            exchangeFactory.Create();
        }

        void CreateProductionRegistry()
        {
            var productionRegistryFactory =
                new ProductionRegistryFactory(TypeObject.ProductionRegistry.Value, GameObject);
            var productionRegistryComponent = productionRegistryFactory.Create();

            productionRegistryComponent.RecipeFinished += Component.OnRecipeFinished;
        }

        void SetupCompany()
        {
            var companyComponent = GameObject.GetComponent<CompanyComponent>();
            companyComponent.EmployeeArrived += Component.OnEmployeeArrived;
        }

        void SetupResourceInventory()
        {
            var resourceInventoryComponent = GameObject.GetComponent<InventoryComponent>();
            resourceInventoryComponent.Added += Component.OnItemAdded;
        }

        void SetupTime()
        {
            if (Locator.Time) Locator.Time.Evening += Component.OnEvening;
        }
    }
}