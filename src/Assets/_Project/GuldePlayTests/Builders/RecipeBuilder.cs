using System.Collections.Generic;
using System.Linq;
using Gulde.Economy;
using Gulde.Logging;
using Gulde.Production;
using Sirenix.Utilities;
using UnityEngine;

namespace GuldePlayTests.Builders
{
    public class RecipeBuilder
    {
        Recipe Recipe { get; }

        public RecipeBuilder() => Recipe = ScriptableObject.CreateInstance<Recipe>();

        public RecipeBuilder WithName(string name)
        {
            var type = Recipe.GetType();
            var nameProperty = type.GetProperty("Name");

            nameProperty?.SetValue(Recipe, name);

            return this;
        }

        public RecipeBuilder WithResources(Dictionary<Item, int> resources)
        {
            var type = Recipe.GetType();
            var resourcesProperty = type.GetProperty("Resources");

            resourcesProperty?.SetValue(Recipe, resources);

            return this;
        }

        public RecipeBuilder WithProduct(Item product)
        {
            var type = Recipe.GetType();
            var productProperty = type.GetProperty("Product");

            productProperty?.SetValue(Recipe, product);

            return this;
        }

        public RecipeBuilder WithExternality(bool isExternal)
        {
            var type = Recipe.GetType();
            var externalityProperty = type.GetProperty("IsExternal");

            externalityProperty?.SetValue(Recipe, isExternal);

            return this;
        }

        public RecipeBuilder WithTime(int time)
        {
            var type = Recipe.GetType();
            var timeProperty = type.GetProperty("Time");

            timeProperty?.SetValue(Recipe, time);

            return this;

        }

        public Recipe Build()
        {
            if (Recipe.Name.IsNullOrWhitespace())
            {
                this.Log($"Cannot create recipe with invalid name \"{Recipe.Name}\"", LogType.Error);
                return null;
            }

            if (Recipe.Resources != null && Recipe.Resources.Any(e => e.Value <= 0))
            {
                this.Log($"Cannot create recipe with invalid resources {Recipe.Resources}", LogType.Error);
                return null;
            }

            if (Recipe.Product == null)
            {
                this.Log($"Cannot create recipe with invalid product {Recipe.Product}", LogType.Error);
                return null;
            }

            if (Recipe.Time < 0)
            {
                this.Log($"Cannot create recipe with invalid time {Recipe.Time}", LogType.Error);
                return null;
            }

            return Recipe;
        }
    }
}