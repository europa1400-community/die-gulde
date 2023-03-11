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
            Assert.IsTrue(inventory.HasItem(item, 1));
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
        public void ShouldAddItemToSlot()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);
            Assert.IsTrue(inventory.HasItem(item, 1));
        }
        
        [Test]
        public void ShouldAddItem()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, 1);
            
            Assert.IsTrue(wasAdded);
            Assert.IsTrue(inventory.HasItem(item, 1));
        }
        
        [Test]
        public void ShouldAddItemToMultipleSlots()
        {
            var slot2 = new InventorySlot();
            
            inventory.slots.Add(slot2);
            
            var wasRegistered = inventory.Register(item, slot);
            var wasRegistered2 = inventory.Register(item, slot2);

            Assert.IsTrue(wasRegistered);
            Assert.IsTrue(wasRegistered2);

            var wasAdded = inventory.Add(item, 2);
            
            Assert.IsTrue(wasAdded);
            Assert.IsTrue(inventory.HasItem(item, 2));
            Assert.AreEqual(1, slot.supply);
            Assert.AreEqual(1, slot2.supply);
        }
        
        [Test]
        public void ShouldAddItemToUnregisteredSlots()
        {
            var slot2 = new InventorySlot();
            var slot3 = new InventorySlot();
            
            inventory.slots.Add(slot2);
            inventory.slots.Add(slot3);
            
            var wasRegistered = inventory.Register(item, slot);

            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, 2);
            
            Assert.IsTrue(wasAdded);
            Assert.IsTrue(inventory.HasItem(item, 2));
            Assert.AreEqual(1, slot.supply);
            Assert.AreEqual(1, slot2.supply);
            Assert.AreEqual(0, slot3.supply);
        }
        
        [Test]
        public void ShouldAddItemToNullSlot()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, null, 1);
            
            Assert.IsTrue(wasAdded);
            Assert.IsTrue(inventory.HasItem(item, 1));
        }
        
        [Test]
        public void ShouldNotOverflowAddItemToOccupiedSlots()
        {
            var slot2 = new InventorySlot();
            var item2 = ScriptableObject.CreateInstance<Item>();
            
            inventory.slots.Add(slot2);
            
            var wasRegistered = inventory.Register(item, slot);
            var wasRegistered2 = inventory.Register(item2, slot2);

            Assert.IsTrue(wasRegistered);
            Assert.IsTrue(wasRegistered2);

            var wasAdded = inventory.Add(item, 2);
            
            Assert.IsFalse(wasAdded);
            Assert.IsFalse(inventory.HasItem(item, 1));
            Assert.AreEqual(0, slot.supply);
            Assert.AreEqual(0, slot2.supply);
        }
        
        [Test]
        public void ShouldAddItemToFullestSlot()
        {
            var slot2 = new InventorySlot();
            
            inventory.slots.Add(slot2);
            inventory.stackSize = 5;
            
            var wasRegistered = inventory.Register(item, slot);
            var wasRegistered2 = inventory.Register(item, slot2);

            Assert.IsTrue(wasRegistered);
            Assert.IsTrue(wasRegistered2);

            var wasAdded = inventory.Add(item, slot, 2);
            var wasAdded2 = inventory.Add(item, slot2, 1);
            
            Assert.IsTrue(wasAdded);
            Assert.IsTrue(wasAdded2);

            var wasAdded3 = inventory.Add(item, 3);

            Assert.IsTrue(wasAdded3);
            Assert.IsTrue(inventory.HasItem(item, 6));
            Assert.AreEqual(5, slot.supply);
            Assert.AreEqual(1, slot2.supply);
        }
        
        [Test]
        public void ShouldNotAddItemToFullSlot()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded1 = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded1);

            var wasAdded2 = inventory.Add(item, slot, 1);
            
            Assert.IsFalse(wasAdded2);
            Assert.IsTrue(inventory.HasItem(item, 1));
        }
        
        [Test]
        public void ShouldNotAddWrongItemToSlot()
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
        public void ShouldRemoveItemFromSlot()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);

            var wasRemoved = inventory.Remove(item, slot, 1);
            
            Assert.IsTrue(wasRemoved);
            Assert.IsFalse(inventory.HasItem(item, 1));
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.supply);
        }
        
        [Test]
        public void ShouldRemoveItemsFromMultipleSlots()
        {
            var slot2 = new InventorySlot();
            
            inventory.slots.Add(slot2);
            inventory.stackSize = 3;
            
            var wasRegistered = inventory.Register(item, slot);
            var wasRegistered2 = inventory.Register(item, slot2);
            
            Assert.IsTrue(wasRegistered);
            Assert.IsTrue(wasRegistered2);

            var wasAdded = inventory.Add(item, slot, 3);
            var wasAdded2 = inventory.Add(item, slot2, 2);
            
            Assert.IsTrue(wasAdded);
            Assert.IsTrue(wasAdded2);

            var wasRemoved = inventory.Remove(item, 5);
            
            Assert.IsTrue(wasRemoved);
            Assert.IsFalse(inventory.HasItem(item, 1));
            Assert.AreEqual(0, slot.supply);
            Assert.AreEqual(0, slot2.supply);
        }
        
        [Test]
        public void ShouldRemoveItemsFromEmptierSlot()
        {
            var slot2 = new InventorySlot();
            
            inventory.slots.Add(slot2);
            inventory.stackSize = 3;
            
            var wasRegistered = inventory.Register(item, slot);
            var wasRegistered2 = inventory.Register(item, slot2);
            
            Assert.IsTrue(wasRegistered);
            Assert.IsTrue(wasRegistered2);

            var wasAdded = inventory.Add(item, slot, 3);
            var wasAdded2 = inventory.Add(item, slot2, 2);
            
            Assert.IsTrue(wasAdded);
            Assert.IsTrue(wasAdded2);

            var wasRemoved = inventory.Remove(item, 2);
            
            Assert.IsTrue(wasRemoved);
            Assert.IsTrue(inventory.HasItem(item, 3));
            Assert.AreEqual(3, slot.supply);
            Assert.AreEqual(0, slot2.supply);
        }
        
        [Test]
        public void ShouldRemoveItemFromNullSlot()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);

            var wasRemoved = inventory.Remove(item, null, 1);
            
            Assert.IsTrue(wasRemoved);
            Assert.IsFalse(inventory.HasItem(item, 1));
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.supply);
        }
        
        [Test]
        public void ShouldNotRemoveWrongItemFromSlot()
        {
            var item2 = ScriptableObject.CreateInstance<Item>();
            
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);

            var wasRemoved = inventory.Remove(item2, slot, 1);
            
            Assert.IsFalse(wasRemoved);
            Assert.IsTrue(inventory.HasItem(item, 1));
        }
        
        [Test]
        public void ShouldNotRemoveBelowZeroFromSlot()
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
        public void ShouldNotRemoveBelowZero()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, slot, 1);
            
            Assert.IsTrue(wasAdded);

            var wasRemoved1 = inventory.Remove(item, 1);
            
            Assert.IsTrue(wasRemoved1);

            var wasRemoved2 = inventory.Remove(item, 1);
            
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
            Assert.IsTrue(inventory.HasItem(item, 2));
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
            Assert.IsTrue(inventory.HasItem(item, 1));
        }
        
        [Test]
        public void ShouldHaveItem()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, 1);
            
            Assert.IsTrue(wasAdded);
            Assert.IsTrue(inventory.HasItem(item, 1));
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(1, slot.supply);
        }
        
        [Test]
        public void ShouldHaveMultipleItems()
        {
            var slot2 = new InventorySlot();
            var item2 = ScriptableObject.CreateInstance<Item>();

            inventory.slots.Add(slot2);
            inventory.stackSize = 2;

            var wasRegistered = inventory.Register(item, slot);
            var wasRegistered2 = inventory.Register(item2, slot2);
            
            Assert.IsTrue(wasRegistered);

            var wasAdded = inventory.Add(item, 1);
            var wasAdded2 = inventory.Add(item2, 2);
            
            Assert.IsTrue(wasAdded);
            Assert.IsTrue(wasAdded2);
            Assert.IsTrue(inventory.HasItem(item, 1));
            Assert.IsTrue(inventory.HasItem(item2, 2));
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(1, slot.supply);
            Assert.AreEqual(item2, slot2.item);
            Assert.AreEqual(2, slot2.supply);
        }
        
        [Test]
        public void ShouldNotHaveNotAddedItem()
        {
            var wasRegistered = inventory.Register(item, slot);
            
            Assert.IsTrue(wasRegistered);
            
            Assert.IsFalse(inventory.HasItem(item, 1));
            Assert.AreEqual(item, slot.item);
            Assert.AreEqual(0, slot.supply);
        }
    }
}