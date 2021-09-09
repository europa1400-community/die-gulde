using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GuldeLib.Economy;
using MonoLogger.Runtime;
using GuldeLib.Production;
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

            Recipe.name = name;

            return this;
        }

        public RecipeBuilder WithResource(Item resource, int amount)
        {
            if (!Recipe.Resources.ContainsKey(resource)) Recipe.Resources.Add(resource, 0);
            Recipe.Resources[resource] = amount;

            return this;
        }

        public RecipeBuilder WithResources(Dictionary<Item, int> resources)
        {
            foreach (var pair in resources)
            {
                if (!Recipe.Resources.ContainsKey(pair.Key)) Recipe.Resources.Add(pair.Key, 0);
                Recipe.Resources[pair.Key] = pair.Value;
            }

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