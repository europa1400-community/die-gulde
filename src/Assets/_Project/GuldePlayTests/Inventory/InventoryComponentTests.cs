using GuldeLib.Factories;
using GuldeLib.Inventories;
using GuldeLib.TypeObjects;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace GuldePlayTests.Inventory
{
    public class InventoryComponentTests
    {
        InventoryComponent InventoryComponent { get; set; }
        Item Resource { get; set; }

        bool InitializedFlag { get; set; }
        bool RegisteredFlag { get; set; }
        bool UnregisteredFlag { get; set; }
        bool AddedFlag { get; set; }
        bool RemovedFlag { get; set; }

        InventoryComponent.InitializedEventArgs InitializedEventArgs { get; set; }
        InventoryComponent.AddedEventArgs AddedEventArgs { get; set; }
        InventoryComponent.RegisteredEventArgs RegisteredEventArgs { get; set; }

        [SetUp]
        public void Setup()
        {
            Resource = An.Item;
            var inventory = An.Inventory;
            var inventoryFactory = new InventoryFactory(inventory, startInactive: true);

            InventoryComponent = inventoryFactory.Create();

            InventoryComponent.Initialized += OnInitialized;
            InventoryComponent.Registered += OnRegistered;
            InventoryComponent.Unregistered += OnUnregistered;
            InventoryComponent.Added += OnAdded;
            InventoryComponent.Removed += OnRemoved;

            InventoryComponent.gameObject.SetActive(true);
        }

        [TearDown]
        public void Teardown()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                Object.DestroyImmediate(gameObject);
            }

            InitializedFlag = false;
            RegisteredFlag = false;
            UnregisteredFlag = false;
            AddedFlag = false;
            RemovedFlag = false;

            InitializedEventArgs = null;
            RegisteredEventArgs = null;
            AddedEventArgs = null;
        }

        [Test]
        public void ShouldInitialize()
        {
            Assert.True(InitializedFlag);
        }

        [Test]
        public void ShouldStartEmpty()
        {
            Assert.True(InventoryComponent.IsEmpty);
        }

        [Test]
        public void ShouldRegister()
        {
            InventoryComponent.Register(Resource);

            Assert.True(InventoryComponent.IsRegistered(Resource));
            Assert.True(RegisteredFlag);
            Assert.AreEqual(Resource, RegisteredEventArgs.Item);
        }

        [Test]
        public void ShouldNotRegisterTwice()
        {
            InventoryComponent.MaxSlots = 2;

            InventoryComponent.Register(Resource);
            InventoryComponent.Register(Resource);

            Assert.False(InventoryComponent.IsFull);
        }

        [Test]
        public void ShouldRegisterWhenNotFull()
        {
            InventoryComponent.Register(Resource);
            Assert.True(InventoryComponent.IsRegistered(Resource));
        }

        [Test]
        public void ShouldRegisterWhenEmptySlotAndCanAutoUnregister()
        {
            var resource2 = An.Item;
            InventoryComponent.AllowAutoUnregister = true;

            InventoryComponent.Register(Resource);
            InventoryComponent.Register(resource2);

            Assert.False(InventoryComponent.IsRegistered(Resource));
            Assert.True(InventoryComponent.IsRegistered(resource2));
        }

        [Test]
        public void ShouldNotRegisterWhenFullAndCanAutoUnregister()
        {
            var resource2 = An.Item;
            InventoryComponent.AllowAutoUnregister = true;

            InventoryComponent.Add(Resource);
            InventoryComponent.Register(resource2);

            Assert.True(InventoryComponent.IsRegistered(Resource));
            Assert.False(InventoryComponent.IsRegistered(resource2));
        }

        [Test]
        public void ShouldNotRegisterWhenFull()
        {
            var resource2 = An.Item;

            InventoryComponent.Add(Resource);
            InventoryComponent.Register(resource2);

            Assert.True(InventoryComponent.IsRegistered(Resource));
            Assert.False(InventoryComponent.IsRegistered(resource2));
        }

        [Test]
        public void ShouldUnregister()
        {
            InventoryComponent.Register(Resource);
            Assert.True(InventoryComponent.IsRegistered(Resource));

            InventoryComponent.Unregister(Resource);
            Assert.False(InventoryComponent.IsRegistered(Resource));
            Assert.True(UnregisteredFlag);
            Assert.AreEqual(Resource, RegisteredEventArgs.Item);
        }

        [Test]
        public void ShouldNotUnregisterWhenNotRegistered()
        {
            Assert.False(InventoryComponent.IsRegistered(Resource));

            InventoryComponent.Unregister(Resource);
            Assert.False(InventoryComponent.IsRegistered(Resource));
        }

        [Test]
        public void ShouldAdd()
        {
            InventoryComponent.Add(Resource);
            InventoryComponent.Add(Resource);

            Assert.AreEqual(2, InventoryComponent.GetSupply(Resource));
            Assert.True(AddedFlag);
            Assert.AreEqual(Resource, AddedEventArgs.Item);
            Assert.AreEqual(1, AddedEventArgs.Amount);
        }

        [Test]
        public void ShouldAddMultiple()
        {
            InventoryComponent.Add(Resource, 3);
            Assert.AreEqual(3, InventoryComponent.GetSupply(Resource));

            InventoryComponent.Add(Resource, 2);
            Assert.AreEqual(5, InventoryComponent.GetSupply(Resource));
        }

        [Test]
        public void ShouldNotAddWhenFull()
        {
            var resource2 = An.Item;

            InventoryComponent.Add(Resource);
            AddedFlag = false;
            InventoryComponent.Add(resource2);

            Assert.True(InventoryComponent.IsRegistered(Resource));
            Assert.False(InventoryComponent.IsRegistered(resource2));
            Assert.False(AddedFlag);
        }

        [Test]
        public void ShouldNotAddWhenExceedingMaxIncrease()
        {
            InventoryComponent.MaxCapacity = 10;

            InventoryComponent.Add(Resource, 5);

            AddedFlag = false;
            InventoryComponent.Add(Resource, 6);

            Assert.False(AddedFlag);
            Assert.AreEqual(5, InventoryComponent.GetSupply(Resource));
        }

        [Test]
        public void ShouldGiveMaxIncrease()
        {
            InventoryComponent.MaxCapacity = 10;
            InventoryComponent.Add(Resource, 4);

            Assert.AreEqual(6, InventoryComponent.MaxIncrease(Resource));
        }

        [Test]
        public void ShouldRemove()
        {
            InventoryComponent.Add(Resource, 7);
            InventoryComponent.Remove(Resource, 3);

            Assert.True(RemovedFlag);
            Assert.AreEqual(4, InventoryComponent.GetSupply(Resource));
            Assert.AreEqual(Resource, AddedEventArgs.Item);
            Assert.AreEqual(3, AddedEventArgs.Amount);
        }

        [Test]
        public void ShouldNotRemoveWhenNotRegistered()
        {
            InventoryComponent.Remove(Resource);
            Assert.False(RemovedFlag);
            Assert.False(InventoryComponent.IsRegistered(Resource));
        }

        [Test]
        public void ShouldNotRemoveWhenExceedingMaxDecrease()
        {
            InventoryComponent.Add(Resource, 5);
            InventoryComponent.Remove(Resource, 6);

            Assert.False(RemovedFlag);
            Assert.AreEqual(5, InventoryComponent.GetSupply(Resource));
        }

        [Test]
        public void ShouldGiveMaxDecrease()
        {
            InventoryComponent.Add(Resource, 6);
            Assert.AreEqual(6, InventoryComponent.MaxDecrease(Resource));
        }

        [Test]
        public void ShouldRemoveAndAutoUnregisterWhenEnabled()
        {
            InventoryComponent.AllowAutoUnregister = true;

            InventoryComponent.Add(Resource);
            Assert.True(InventoryComponent.IsRegistered(Resource));

            InventoryComponent.Remove(Resource);
            Assert.False(InventoryComponent.IsRegistered(Resource));
            Assert.True(UnregisteredFlag);
        }

        [Test]
        public void ShouldAddResources()
        {
            var resource2 = An.Item;
            var recipe = A.Recipe
                .WithResource(Resource)
                .WithResource(resource2, 2);

            InventoryComponent.MaxSlots = 2;
            InventoryComponent.AddResources(recipe);

            Assert.True(AddedFlag);
            Assert.AreEqual(1, InventoryComponent.GetSupply(Resource));
            Assert.AreEqual(2, InventoryComponent.GetSupply(resource2));
        }

        [Test]
        public void ShouldRemoveResources()
        {
            var resource2 = An.Item;
            var recipe = A.Recipe
                .WithResource(Resource)
                .WithResource(resource2, 2);

            InventoryComponent.MaxSlots = 2;
            InventoryComponent.AddResources(recipe);
            InventoryComponent.RemoveResources(recipe);

            Assert.True(RemovedFlag);
            Assert.AreEqual(0, InventoryComponent.GetSupply(Resource));
            Assert.AreEqual(0, InventoryComponent.GetSupply(resource2));
        }

        [Test]
        public void ShouldGiveHasResources()
        {
            var resource2 = An.Item;
            var recipe = A.Recipe
                .WithResource(Resource)
                .WithResource(resource2, 2);

            InventoryComponent.MaxSlots = 2;
            InventoryComponent.AddResources(recipe);

            Assert.True(InventoryComponent.HasResources(recipe));
            Assert.False(InventoryComponent.HasResources(recipe, 2));
        }

        [Test]
        public void ShouldGiveCanAddProduct()
        {
            var resource2 = An.Item;
            var recipe = A.Recipe
                .WithProduct(resource2);

            InventoryComponent.MaxCapacity = 1;

            Assert.True(InventoryComponent.CanAddProduct(recipe));
            Assert.False(InventoryComponent.CanAddProduct(recipe, 2));
        }

        void OnInitialized(object sender, InventoryComponent.InitializedEventArgs e)
        {
            InitializedFlag = true;
            InitializedEventArgs = e;
        }

        void OnRegistered(object sender, InventoryComponent.RegisteredEventArgs e)
        {
            RegisteredFlag = true;
            RegisteredEventArgs = e;
        }

        void OnUnregistered(object sender, InventoryComponent.RegisteredEventArgs e)
        {
            UnregisteredFlag = true;
            RegisteredEventArgs = e;
        }

        void OnAdded(object sender, InventoryComponent.AddedEventArgs e)
        {
            AddedFlag = true;
            AddedEventArgs = e;
        }

        void OnRemoved(object sender, InventoryComponent.AddedEventArgs e)
        {
            RemovedFlag = true;
            AddedEventArgs = e;
        }
    }
}