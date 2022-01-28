using GuldeLib.Economy;
using GuldeLib.Generators;
using GuldeLib.Producing;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class ProductionBuilder : Builder<Production>
    {
        public ProductionBuilder WithExchange(GeneratableExchange exchange)
        {
            Object.Exchange = exchange;
            return this;
        }

        public ProductionBuilder WithAssignment(GeneratableAssignment assignment)
        {
            Object.Assignment = assignment;
            return this;
        }

        public ProductionBuilder WithProductionRegistry(GeneratableProductionRegistry productionRegistry)
        {
            Object.ProductionRegistry = productionRegistry;
            return this;
        }
    }
}