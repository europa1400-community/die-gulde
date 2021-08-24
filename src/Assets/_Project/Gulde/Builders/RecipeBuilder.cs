using System.Collections.Generic;
using Gulde.Economy;
using Gulde.Production;
using UnityEngine;

namespace Gulde.Builders
{
    public class RecipeBuilder
    {
        Recipe Recipe { get; }

        public RecipeBuilder()
        {
            Recipe = ScriptableObject.CreateInstance<Recipe>();
        }

        public RecipeBuilder WithResources(Dictionary<Item, int> resources)
        {
            Recipe.Resources = resources;

            return this;
        }

        public RecipeBuilder WithProduct(Item product)
        {
            Recipe.Product = product;

            return this;
        }

        public RecipeBuilder WithTime(int time)
        {
            Recipe.Time = time;

            return this;

        }

        public RecipeBuilder WithExternality(bool isExternal)
        {
            Recipe.IsExternal = isExternal;

            return this;
        }

        public RecipeBuilder WithName(string name)
        {
            Recipe.name = name;
            return this;
        }

        public Recipe Build() => Recipe;
    }
}