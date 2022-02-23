// using System.Reflection;
// using GuldeLib.Producing;
// using GuldeLib.TypeObjects;
// using GuldePlayTests;
// using NUnit.Framework;
//
// namespace GuldeTests.Production
// {
//     public class RecipeTests
//     {
//         [Test]
//         public void ShouldGetMeanProfitPerHour()
//         {
//             var resource = An.Item
//                 .WithName("resource")
//                 .WithItemType(Item.ItemType.Resource)
//                 .WithMeanPrice(100f)
//                 .WithMinPrice(50f)
//                 .WithMeanSupply(10)
//                 .Build();
//             var product = An.Item
//                 .WithName("product")
//                 .WithItemType(Item.ItemType.Product)
//                 .WithMeanPrice(1000f)
//                 .WithMinPrice(50f)
//                 .WithMeanSupply(10)
//                 .Build();
//             var recipe = A.Recipe
//                 .WithName("recipe")
//                 .WithExternality(false)
//                 .WithResource(resource, 1)
//                 .WithProduct(product)
//                 .WithTime(10)
//                 .Build();
//
//             var expected = (1000f - 100f) / 10f;
//
//             Assert.AreEqual(expected, recipe.MeanProfitPerHour);
//         }
//
//         [Test]
//         public void ShouldNotGetInvalidMeanProfitPerHour()
//         {
//             var resource = An.Item
//                 .WithName("resource")
//                 .WithItemType(Item.ItemType.Resource)
//                 .WithMeanPrice(100f)
//                 .WithMinPrice(50f)
//                 .WithMeanSupply(10)
//                 .Build();
//             var product = An.Item
//                 .WithName("product")
//                 .WithItemType(Item.ItemType.Product)
//                 .WithMeanPrice(1000f)
//                 .WithMinPrice(50f)
//                 .WithMeanSupply(10)
//                 .Build();
//             var recipe = A.Recipe
//                 .WithName("recipe")
//                 .WithExternality(false)
//                 .WithResource(resource, 1)
//                 .WithProduct(product)
//                 .WithTime(10)
//                 .Build();
//
//             var productProperty = recipe.GetType().GetProperty("Product", BindingFlags.Instance | BindingFlags.Public |
//                                                                           BindingFlags.NonPublic);
//             productProperty?.SetValue(recipe, null);
//
//             Assert.AreEqual(0f, recipe.MeanProfitPerHour);
//         }
//
//     }
// }