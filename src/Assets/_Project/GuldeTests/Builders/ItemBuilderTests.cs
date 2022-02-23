// using GuldeLib.Builders;
// using GuldeLib.Economy;
// using GuldeLib.Producing;
// using GuldeLib.TypeObjects;
// using GuldePlayTests;
// using NUnit.Framework;
// using UnityEngine.TestTools;
//
// namespace GuldeTests.Builders
// {
//     public class ItemBuilderTests
//     {
//         ItemBuilder ItemBuilder { get; set; }
//         Item Item { get; set; }
//
//         [SetUp]
//         public void Setup()
//         {
//             ItemBuilder = An.Item
//                 .WithName("item")
//                 .WithItemType(Item.ItemType.Resource)
//                 .WithMeanPrice(10)
//                 .WithMinPrice(1)
//                 .WithMeanSupply(10);
//         }
//
//         [TearDown]
//         public void Teardown()
//         {
//             ItemBuilder = null;
//             Item = null;
//
//             LogAssert.ignoreFailingMessages = false;
//         }
//
//         [Test]
//         public void ShouldBuildItem()
//         {
//             Item = ItemBuilder.Build();
//
//             Assert.NotNull(Item);
//             Assert.AreEqual("item", Item.Name);
//             Assert.AreEqual(Item.ItemType.Resource, Item.Type);
//             Assert.AreEqual(10, Item.MeanPrice);
//             Assert.AreEqual(1, Item.MinPrice);
//             Assert.AreEqual(10, Item.MeanSupply);
//         }
//
//         [Test]
//         public void ShouldNotBuildItemWithInvalidName()
//         {
//             LogAssert.ignoreFailingMessages = true;
//
//             Item = ItemBuilder
//                 .WithName(null)
//                 .Build();
//
//             Assert.IsNull(Item);
//         }
//
//         [Test]
//         public void ShouldNotBuildItemWithInvalidMeanPrice()
//         {
//             LogAssert.ignoreFailingMessages = true;
//
//             Item = ItemBuilder
//                 .WithMeanPrice(-1)
//                 .Build();
//
//             Assert.IsNull(Item);
//         }
//
//         [Test]
//         public void ShouldNotBuildItemWithInvalidMinPrice()
//         {
//             LogAssert.ignoreFailingMessages = true;
//
//             Item = ItemBuilder
//                 .WithMinPrice(-1)
//                 .Build();
//
//             Assert.IsNull(Item);
//         }
//
//         [Test]
//         public void ShouldNotBuildItemWithInvalidMeanSupply()
//         {
//             LogAssert.ignoreFailingMessages = true;
//
//             Item = ItemBuilder
//                 .WithMeanSupply(-1)
//                 .Build();
//
//             Assert.IsNull(Item);
//         }
//     }
// }