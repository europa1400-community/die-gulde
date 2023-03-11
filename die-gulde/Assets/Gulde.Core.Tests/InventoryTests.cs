using System.Collections.Generic;
using Gulde.Core.Inventory;
using NUnit.Framework;
using UnityEngine;

namespace Gulde.Core.Tests
{
    public class InventoryTests
    {
        InventoryComponent inventory;
        InventorySlot slot;
        Item item;
        
        [SetUp]
        public void Setup()
        {
            var gameObject = new GameObject();
            inventory = gameObject.AddComponent<InventoryComponent>();
            slot = new InventorySlot();
            item = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;
        }
        
        [Test]
        public void ShouldRegisterItem()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.supply);
        }
        
        [Test]
        public void ShouldNotRegisterItemWhenSlotOccupied()
        {
            var wasRegistered1 = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered1);
            
            var wasRegistered2 = inventory.Register(item, slot);
            
            Assert.IsFalse(wasRegistered2);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.supply);
        }
        
        [Test]
        public void ShouldRegisterItemWithFlags()
        {
            item.itemFlags.Add(ItemFlag.Processable);
            inventory.itemFlags.Add(ItemFlag.Processable);

            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.supply);
        }
        
        [Test]
        public void ShouldNotRegisterItemWithWrongFlags()
        {
            item.itemFlags.Add(ItemFlag.Producible);
            inventory.itemFlags.Add(ItemFlag.Processable);
            inventory.itemFlags.Add(ItemFlag.Consumable);

            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsFalse(wasRegistered);
            Assert.IsNull(slot.item);
            Assert.AreEqual(0, slot.supply);
        }
        
        [Test]
        public void ShouldUnregisterItem()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasUnregistered = inventory.Unregister(slot);
            
            Assert.IsTrue(wasUnregistered);
            Assert.IsNull(slot.item);
            Assert.AreEqual(0, slot.supply);
        }
        
        [Test]
        public void ShouldNotUnregisterFilledSlot()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);
            
            var wasAdded = inventory.Add(item, slot);
            
            Assert.IsTrue(wasAdded);

            var wasUnregistered = inventory.Unregister(slot);
            
            Assert.IsFalse(wasUnregistered);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(1, slot.supply);
        }
        
        [Test]
        public void ShouldNotUnregisterUnregisteredSlot()
        {
            var wasUnregistered = inventory.Unregister(slot);
            
            Assert.IsFalse(wasUnregistered);
            Assert.IsNull(slot.item);
            Assert.AreEqual(0, slot.supply);
        }
        
        [Test]
        public void ShouldAddItem()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(1, slot.supply);
        }
        
        [Test]
        public void ShouldNotAddItemWhenFull()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded1 = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded1);

            var wasAdded2 = inventory.Add(item, slot, 1);
            
            Assert.IsFalse(wasAdded2);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(1, slot.supply);
        }
        
        [Test]
        public void ShouldNotAddWrongItem()
        {
            var item2 = ScriptableObject.CreateInstance<Item>();
            
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item2, slot, 1);
            
            Assert.IsFalse(wasAdded);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.supply);
        }
        
        [Test]
        public void ShouldRemoveItem()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);

            var wasRemoved = inventory.Remove(item, slot, 1);
            
            Assert.IsTrue(wasRemoved);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.supply);
        }
        
        [Test]
        public void ShouldNotRemoveWrongItem()
        {
            var item2 = ScriptableObject.CreateInstance<Item>();
            
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);

            var wasRemoved = inventory.Remove(item2, slot, 1);
            
            Assert.IsFalse(wasRemoved);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(1, slot.supply);
        }
        
        [Test]
        public void ShouldNotRemoveBelowZero()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);

            var wasRemoved1 = inventory.Remove(item, slot, 1);
            
            Assert.IsTrue(wasRemoved1);

            var wasRemoved2 = inventory.Remove(item, slot, 1);
            
            Assert.IsFalse(wasRemoved2);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.supply);
        }
        
        [Test]
        public void ShouldAddMultipleItemsToSlot()
        {
            inventory.stackSize = 2;
            var wasRegistered = inventory.Register(item, slot);

            Assert.IsTrue(wasRegistered);

            var wasAdded1 = inventory.Add(item, slot);

            Assert.IsTrue(wasAdded1);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(1, slot.supply);

            var wasAdded2 = inventory.Add(item, slot);

            Assert.IsTrue(wasAdded2);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(2, slot.supply);
        }
        
        [Test]
        public void ShouldNotRemoveMoreItemsThanAvailable()
        {
            var wasRegistered = inventory.Register(item, slot);

            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot);

            Assert.IsTrue(wasAdded);

            var wasRemoved = inventory.Remove(item, slot, 2);

            Assert.IsFalse(wasRemoved);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(1, slot.supply);
        }
    }
}