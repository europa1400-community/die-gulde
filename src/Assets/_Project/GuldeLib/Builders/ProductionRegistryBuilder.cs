using System.Collections.Generic;
using GuldeLib.Producing;
using GuldeLib.TypeObjects;

namespace GuldeLib.Builders
{
    public class ProductionRegistryBuilder : Builder<ProductionRegistry>
    {
        public ProductionRegistryBuilder WithRecipes(HashSet<Recipe> recipes)
        {
            Object.Recipes = recipes;
            return this;
        }
    }
}