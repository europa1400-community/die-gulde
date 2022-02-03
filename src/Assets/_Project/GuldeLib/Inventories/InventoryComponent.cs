using System;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.TypeObjects;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Inventories
{
    public class InventoryComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        public int MaxCapacity { get; set; }

        [ShowInInspector]
        public int MaxSlots { get; set; }

        [ShowInInspector]
        public bool AllowAutoUnregister { get; set; }

        [ShowInInspector]
        [TableList]
        public List<Slot> Slots { get; set; } = new List<Slot>();

        public int FreeSlots =>
            MaxSlots - Slots.Count;

        public bool IsFull =>
            FreeSlots <= 0;

        public bool IsEmpty =>
            Slots != null && FreeSlots == MaxSlots;

        public Slot FirstRemovableSlot =>
            Slots.FirstOrDefault(e => e.Supply <= 0);

        public List<Slot> RemovableSlots =>
            Slots.Where(e => e.Supply <= 0).ToList();

        public bool CanRemoveSlot(Slot slot = null) =>
            AllowAutoUnregister &&
            (slot != null ? RemovableSlots.Contains(slot) : FirstRemovableSlot != null);

        public Slot GetSlot(Item item) =>
            Slots.FirstOrDefault(e => e.Item == item);

        public int GetSupply(Item item) =>
            GetSlot(item)?.Supply ?? 0;

        public bool IsRegistered(Item item) =>
            GetSlot(item) != null;

        public bool IsInStock(Item item, int amount = 1) =>
            GetSupply(item) >= amount;

        public bool HasAvailableSlot =>
            !IsFull || CanRemoveSlot();

        public bool CanRegister(Item item) =>
            !IsRegistered(item) && HasAvailableSlot;

        public bool CanOrIsRegistered(Item item) =>
            IsRegistered(item) || CanRegister(item);

        public bool CanIncreaseSupply(Item item, int amount = 1) =>
            CanOrIsRegistered(item) && GetSupply(item) + amount <= MaxCapacity;

        public int MaxIncrease(Item item) =>
            CanIncreaseSupply(item) ? MaxCapacity - GetSupply(item) : 0;

        public int MaxDecrease(Item item) =>
            GetSupply(item);

        public bool HasResources(Recipe recipe, int amount = 1) =>
            recipe.Resources.All(pair => IsInStock(pair.Key, pair.Value * amount));

        public bool CanAddProduct(Recipe recipe, int amount = 1) =>
            CanIncreaseSupply(recipe.Product, amount);

        void Awake()
        {
            Initialized?.Invoke(this, new InitializedEventArgs());
        }

        public Slot Register(Item item)
        {
            if (IsRegistered(item))
            {
                this.Log($"Cannot register {item}: Already registered");
                return GetSlot(item);
            }

            if (!CanRemoveSlot() && IsFull)
            {
                this.Log($"Cannot register {item}: No free slot available");
                return null;
            }

            this.Log($"Registering {item}");
            var slot = new Slot(item);

            if (!IsFull)
            {
                Slots.Add(slot);
                Registered?.Invoke(this, new RegisteredEventArgs(item));
                return slot;
            }

            Unregister(FirstRemovableSlot.Item);
            Slots.Add(slot);

            Registered?.Invoke(this, new RegisteredEventArgs(item));
            return slot;
        }

        public void Unregister(Item item)
        {
            if (!IsRegistered(item))
            {
                this.Log($"Cannot unregister {item}: Item is not registered");
                return;
            }

            this.Log($"Unregistering {item}");

            Slots.Remove(GetSlot(item));
            Unregistered?.Invoke(this, new RegisteredEventArgs(item));
        }

        public void Add(Item item, int amount = 1)
        {
            if (!IsRegistered(item) && !CanRegister(item))
            {
                this.Log($"Cannot add {amount} {item}: Cannot register item", LogType.Warning);
                return;
            }

            var slot = Register(item);

            if (amount > MaxIncrease(item))
            {
                this.Log($"Cannot add {amount} {item}: Amount exceeds maximum increase");
                return;
            }

            this.Log($"Adding {amount} {item}");

            slot.Supply += amount;
            Added?.Invoke(this, new AddedEventArgs(item, amount));
        }

        public void Remove(Item item, int amount = 1)
        {
            if (!IsRegistered(item))
            {
                this.Log($"Cannot remove {amount} {item}: Item not registered");
                return;
            }

            var slot = GetSlot(item);

            if (amount > MaxDecrease(item))
            {
                this.Log($"Cannot remove {amount} {item}: Amount exceeds maximum decrease");
                return;
            }

            this.Log($"Removing {amount} {item}");

            slot.Supply = Mathf.Max(slot.Supply - amount, 0);
            Removed?.Invoke(this, new AddedEventArgs(item, amount));

            if (AllowAutoUnregister && CanRemoveSlot(slot))
            {
                Unregister(item);
            }
        }

        public void AddResources(Recipe recipe)
        {
            this.Log($"Adding resources for {recipe}");

            foreach (var pair in recipe.Resources)
            {
                var resource = pair.Key;
                var amount = pair.Value;

                Add(resource, amount);
            }
        }

        public void RemoveResources(Recipe recipe)
        {
            this.Log($"Removing resources for {recipe}");

            foreach (var pair in recipe.Resources)
            {
                var resource = pair.Key;
                var amount = pair.Value;

                Remove(resource, amount);
            }
        }

        public event EventHandler<RegisteredEventArgs> Registered;
        public event EventHandler<RegisteredEventArgs> Unregistered;
        public event EventHandler<AddedEventArgs> Added;
        public event EventHandler<AddedEventArgs> Removed;
        public event EventHandler<InitializedEventArgs> Initialized;

        [Serializable]
        public class Slot
        {
            [OdinSerialize]
            public Item Item { get; }

            [OdinSerialize]
            public int Supply { get; set; }

            public Slot(Item item, int supply = 0)
            {
                Item = item;
                Supply = supply;
            }
        }

        public class InitializedEventArgs : EventArgs
        {
        }

        public class AddedEventArgs : EventArgs
        {
            public AddedEventArgs(Item item, int amount)
            {
                Item = item;
                Amount = amount;
            }

            public Item Item { get; }
            public int Amount { get; }
        }

        public class RegisteredEventArgs : EventArgs
        {
            public RegisteredEventArgs(Item item)
            {
                Item = item;
            }

            public Item Item { get; }
        }
    }
}