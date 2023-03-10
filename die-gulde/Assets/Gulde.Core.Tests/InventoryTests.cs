using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Gulde.Core.Tests
{
    public class InventoryTests
    {
        [Test]
        public void ShouldRegisterItem()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;

            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.count);
        }
        
        [Test]
        public void ShouldNotRegisterItemWhenSlotOccupied()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;

            var wasRegistered1 = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered1);
            
            var wasRegistered2 = inventory.Register(item, slot);
            
            Assert.IsFalse(wasRegistered2);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.count);
        }
        
        [Test]
        public void ShouldRegisterItemWithFlags()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;
                
            item.itemFlags.Add(ItemFlag.Processable);
            inventory.itemFlags.Add(ItemFlag.Processable);

            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.count);
        }
        
        [Test]
        public void ShouldNotRegisterItemWithWrongFlags()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;
                
            item.itemFlags.Add(ItemFlag.Producible);
            inventory.itemFlags.Add(ItemFlag.Processable);
            inventory.itemFlags.Add(ItemFlag.Consumable);

            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsFalse(wasRegistered);
            Assert.IsNull(slot.item);
            Assert.AreEqual(0, slot.count);
        }
        
        [Test]
        public void ShouldUnregisterItem()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;

            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasUnregistered = inventory.Unregister(slot);
            
            Assert.IsTrue(wasUnregistered);
            Assert.IsNull(slot.item);
            Assert.AreEqual(0, slot.count);
        }
        
        [Test]
        public void ShouldNotUnregisterFilledSlot()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;

            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);
            
            var wasAdded = inventory.Add(item, slot);
            
            Assert.IsTrue(wasAdded);

            var wasUnregistered = inventory.Unregister(slot);
            
            Assert.IsFalse(wasUnregistered);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(1, slot.count);
        }
        
        [Test]
        public void ShouldNotUnregisterUnregisteredSlot()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;

            var wasUnregistered = inventory.Unregister(slot);
            
            Assert.IsFalse(wasUnregistered);
            Assert.IsNull(slot.item);
            Assert.AreEqual(0, slot.count);
        }
        
        [Test]
        public void ShouldAddItem()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;

            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(1, slot.count);
        }
        
        [Test]
        public void ShouldNotAddItemWhenFull()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;

            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded1 = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded1);

            var wasAdded2 = inventory.Add(item, slot, 1);
            
            Assert.IsFalse(wasAdded2);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(1, slot.count);
        }
        
        [Test]
        public void ShouldNotAddWrongItem()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item1 = ScriptableObject.CreateInstance<Item>();
            var item2 = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;

            var wasRegistered = inventory.Register(item1, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item2, slot, 1);
            
            Assert.IsFalse(wasAdded);
            Assert.AreEqual(item1, slot.item);
            Assert.AreEqual(0, slot.count);
        }
        
        [Test]
        public void ShouldRemoveItem()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;

            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);

            var wasRemoved = inventory.Remove(item, slot, 1);
            
            Assert.IsTrue(wasRemoved);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.count);
        }
        
        [Test]
        public void ShouldNotRemoveWrongItem()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item1 = ScriptableObject.CreateInstance<Item>();
            var item2 = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;

            var wasRegistered = inventory.Register(item1, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item1, slot, 1);
            
            Assert.IsTrue(wasAdded);

            var wasRemoved = inventory.Remove(item2, slot, 1);
            
            Assert.IsFalse(wasRemoved);
            Assert.AreEqual(item1, slot.item);
            Assert.AreEqual(1, slot.count);
        }
        
        [Test]
        public void ShouldNotRemoveBelowZero()
        {
            var gameObject = new GameObject();
            var inventory = gameObject.AddComponent<InventoryComponent>();
            var slot = new InventorySlot();
            var item = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot);
            inventory.stackSize = 1;

            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);

            var wasRemoved1 = inventory.Remove(item, slot, 1);
            
            Assert.IsTrue(wasRemoved1);

            var wasRemoved2 = inventory.Remove(item, slot, 1);
            
            Assert.IsFalse(wasRemoved2);
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.count);
        }
    }
}