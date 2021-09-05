using System.Collections.Generic;
using GuldeLib.Economy;
using GuldeLib.Production;
using GuldePlayTests;
using GuldePlayTests.Builders;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace GuldeTests.Builders
{
    public class RecipeBuilderTests
    {
        RecipeBuilder RecipeBuilder { get; set; }
        Recipe Recipe { get; set; }
        Item Resource { get; set; }
        Item Product { get; set; }

        [SetUp]
        public void Setup()
        {
            Resource = An.Item
                .WithName("resource")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(10)
                .WithMinPrice(1)
                .WithMeanSupply(10)
                .Build();
            Product = An.Item
                .WithName("product")
                .WithItemType(ItemType.Product)
                .WithMeanPrice(10)
                .WithMinPrice(1)
                .WithMeanSupply(10)
                .Build();

            RecipeBuilder = A.Recipe
                .WithName("Recipe")
                .WithResource(Resource, 1)
                .WithProduct(Product)
                .WithExternality(false)
                .WithTime(1);
        }

        [TearDown]
        public void Teardown()
        {
            RecipeBuilder = null;
            Recipe = null;
            Resource = null;
            Product = null;

            LogAssert.ignoreFailingMessages = false;
        }

        [Test]
        public void ShouldBuildInternalRecipe()
        {
            Recipe = RecipeBuilder.Build();
            var resources = new Dictionary<Item, int>() {{Resource, 1}};

            Assert.NotNull(Recipe);
            Assert.AreEqual("Recipe", Recipe.Name);
            Assert.AreEqual(resources, Recipe.Resources);
            Assert.AreEqual(Product, Recipe.Product);
            Assert.IsFalse(Recipe.IsExternal);
            Assert.AreEqual(1, Recipe.Time);
        }

        [Test]
        public void ShouldBuildRecipeWithResources()
        {
            var resources = new Dictionary<Item, int>() {{Resource, 2}};
            Recipe = RecipeBuilder
                .WithResources(resources)
                .Build();

            Assert.AreEqual(resources, Recipe.Resources);
        }

        [Test]
        public void ShouldBuildExternalRecipe()
        {
            Recipe = RecipeBuilder
                .WithExternality(true)
                .Build();

            Assert.NotNull(Recipe);
            Assert.IsTrue(Recipe.IsExternal);
        }

        [Test]
        public void ShouldNotBuildRecipeWithInvalidName()
        {
            LogAssert.ignoreFailingMessages = true;

            Recipe = RecipeBuilder
                .WithName(null)
                .Build();

            Assert.IsNull(Recipe);
        }

        [Test]
        public void ShouldNotBuildRecipeWithInvalidResources()
        {
            var resources = new Dictionary<Item, int> {{Resource, -1}};
            LogAssert.ignoreFailingMessages = true;

            Recipe = RecipeBuilder
                .WithResources(resources)
                .Build();

            Assert.IsNull(Recipe);
        }

        [Test]
        public void ShouldNotBuildRecipeWithInvalidProduct()
        {
            LogAssert.ignoreFailingMessages = true;

            Recipe = RecipeBuilder
                .WithProduct(null)
                .Build();

            Assert.IsNull(Recipe);
        }

        [Test]
        public void ShouldNotBuildRecipeWithInvalidTime()
        {
            LogAssert.ignoreFailingMessages = true;

            Recipe = RecipeBuilder
                .WithTime(-1)
                .Build();

            Assert.IsNull(Recipe);
        }
    }
}