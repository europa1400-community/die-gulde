using GuldeLib.Producing;
using UnityEngine;

namespace GuldeLib.Factories
{
    public class ProductionFactory : Factory<Production>
    {
        public ProductionFactory(GameObject gameObject = null, GameObject parentObject = null) : base(gameObject, parentObject)
        {
        }

        public override GameObject Create(Production production)
        {
            var assignmentFactory = new AssignmentFactory(GameObject);
            assignmentFactory.Create(production.Assignment.Value);

            var exchangeFactory = new ExchangeFactory(GameObject);
            exchangeFactory.Create(production.Exchange.Value);

            var productionRegistryFactory = new ProductionRegistryFactory(GameObject);
            productionRegistryFactory.Create(production.ProductionRegistry.Value);

            var productionComponent = GameObject.AddComponent<ProductionComponent>();

            return GameObject;
        }

        public override GameObject Generate() => null;
    }
}