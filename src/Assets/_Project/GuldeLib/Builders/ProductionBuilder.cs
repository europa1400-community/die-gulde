using GuldeLib.Economy;
using GuldeLib.Producing;

namespace GuldeLib.Builders
{
    public class ProductionBuilder : Builder<Production>
    {
        public ProductionBuilder WithExchange(Exchange exchange)
        {
            Object.Exchange = exchange;
            return this;
        }

        public ProductionBuilder WithAssignment(Assignment assignment)
        {
            Object.Assignment = assignment;
            return this;
        }

        public ProductionBuilder WithProductionRegistry(ProductionRegistry productionRegistry)
        {
            Object.ProductionRegistry = productionRegistry;
            return this;
        }
    }
}