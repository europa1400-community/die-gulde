using System.Collections.Generic;
using System.Linq;
using GuldeLib.Economy;
using GuldeLib.Producing;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using Sirenix.Utilities;
using UnityEngine;

namespace GuldeLib.Builders
{
    public class RecipeBuilder : Builder<Recipe>
    {
        public RecipeBuilder WithName(string name)
        {
            Object.Name = name;
            return this;
        }

        public RecipeBuilder WithResource(Item resource, int amount = 1)
        {
            Object.Resources ??= new Dictionary<Item, int>();
            Object.Resources[resource] = amount;
            return this;
        }

        public RecipeBuilder WithResources(Dictionary<Item, int> resources)
        {
            Object.Resources = resources;
            return this;
        }

        public RecipeBuilder WithProduct(Item product)
        {
            Object.Product = product;
            return this;
        }

        public RecipeBuilder WithExternality(bool isExternal)
        {
            Object.IsExternal = isExternal;
            return this;
        }

        public RecipeBuilder WithTime(int time)
        {
            Object.Time = time;
            return this;
        }
    }
}