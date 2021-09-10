using System.Collections;
using System.Reflection;
using GuldeLib.Builders;
using GuldeLib.Company;
using GuldeLib.Inventory;
using GuldeLib.Production;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GuldePlayTests.Inventory
{
    public class InventoryComponentTests
    {
        GameBuilder GameBuilder { get; set; }
        CityBuilder CityBuilder { get; set; }
        CompanyBuilder CompanyBuilder { get; set; }

        GameObject CompanyObject => CompanyBuilder.CompanyObject;
        CompanyComponent Company => CompanyObject.GetComponent<CompanyComponent>();

        bool ItemRemovedFlag { get; set; }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            CompanyBuilder = A.Company
                .WithSlots(1, 1);
            CityBuilder = A.City
                .WithCompany(CompanyBuilder)
                .WithSize(10, 10)
                .WithWorkerHomes(1)
                .WithNormalTimeSpeed(100);
            GameBuilder = A.Game
                .WithCity(CityBuilder)
                .WithTimeScale(20f);

            yield return GameBuilder.Build();
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            ItemRemovedFlag = false;
        }

        [Test]
        public void ShouldNotRegisterWhenFull()
        {
            var resource1 = An.Item
                .WithName("resource1")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(100f)
                .WithMeanSupply(10)
                .WithMinPrice(50f)
                .Build();
            var resource2 = An.Item
                .WithName("resource2")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(100f)
                .WithMeanSupply(10)
                .WithMinPrice(50f)
                .Build();

            Company.Production.ResourceInventory.Add(resource1);
            Company.Production.ResourceInventory.Register(resource2);

            Assert.True(Company.Production.ResourceInventory.IsRegistered(resource1));
            Assert.False(Company.Production.ResourceInventory.IsRegistered(resource2));
        }

        [Test]
        public void ShouldNotAddWhenFull()
        {
            var resource1 = An.Item
                .WithName("resource1")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(100f)
                .WithMeanSupply(10)
                .WithMinPrice(50f)
                .Build();
            var resource2 = An.Item
                .WithName("resource2")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(100f)
                .WithMeanSupply(10)
                .WithMinPrice(50f)
                .Build();

            Company.Production.ResourceInventory.Add(resource1);
            Company.Production.ResourceInventory.Add(resource2);

            Assert.True(Company.Production.ResourceInventory.IsRegistered(resource1));
            Assert.False(Company.Production.ResourceInventory.IsRegistered(resource2));
            Assert.AreEqual(0, Company.Production.ResourceInventory.GetSupply(resource2));
        }

        [Test]
        public void ShouldBeEmpty()
        {
            Assert.True(Company.Production.ProductInventory.IsEmpty);
        }

        [Test]
        public void ShouldNotRemoveWhenNotRegistered()
        {
            var resource1 = An.Item
                .WithName("resource1")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(100f)
                .WithMeanSupply(10)
                .WithMinPrice(50f)
                .Build();
            var resource2 = An.Item
                .WithName("resource2")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(100f)
                .WithMeanSupply(10)
                .WithMinPrice(50f)
                .Build();

            Company.Production.ResourceInventory.Removed += OnItemRemoved;

            Company.Production.ResourceInventory.Register(resource1);

            Company.Production.ResourceInventory.Remove(resource2);

            Assert.False(ItemRemovedFlag);
            Assert.False(Company.Production.ResourceInventory.IsRegistered(resource2));
        }

        [Test]
        public void ShouldUnregisterOnEmptyWhenEnabled()
        {
            var resource = An.Item
                .WithName("resource")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(100f)
                .WithMeanSupply(10)
                .WithMinPrice(50f)
                .Build();

            Company.Production.ResourceInventory.Add(resource);

            var unregisterProperty = Company.Production.ResourceInventory.GetType().GetProperty("UnregisterWhenEmpty", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            unregisterProperty?.SetValue(Company.Production.ResourceInventory, true);

            Assert.True(Company.Production.ResourceInventory.IsRegistered(resource));

            Company.Production.ResourceInventory.Remove(resource);

            Assert.False(Company.Production.ResourceInventory.IsRegistered(resource));
        }

        [Test]
        public void ShouldNotUnregisterWhenInvalid()
        {
            var resource = An.Item
                .WithName("resource")
                .WithItemType(ItemType.Resource)
                .WithMeanPrice(100f)
                .WithMeanSupply(10)
                .WithMinPrice(50f)
                .Build();

            Assert.False(Company.Production.ResourceInventory.IsRegistered(resource));

            Company.Production.ResourceInventory.Unregister(resource);

            Assert.False(Company.Production.ResourceInventory.IsRegistered(resource));
        }

        void OnItemRemoved(object sender, ItemEventArgs e)
        {
            ItemRemovedFlag = true;
        }
    }
}