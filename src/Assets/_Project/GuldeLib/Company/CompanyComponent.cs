using System;
using System.Collections.Generic;
using GuldeLib.Company.Employees;
using GuldeLib.Economy;
using GuldeLib.Entities;
using GuldeLib.Maps;
using GuldeLib.Production;
using GuldeLib.Timing;
using GuldeLib.Vehicles;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GuldeLib.Company
{
    [RequireComponent(typeof(LocationComponent))]
    public class CompanyComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        public int HiringCost { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        public int CartCost { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        public float WagePerHour { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        GameObject EmployeePrefab { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        GameObject CartPrefab { get; set; }

        [OdinSerialize]
        [BoxGroup("Info")]
        public WealthComponent Owner { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [BoxGroup("Info")]
        public HashSet<EmployeeComponent> Employees { get; set; } = new HashSet<EmployeeComponent>();

        [OdinSerialize]
        [ReadOnly]
        [BoxGroup("Info")]
        public HashSet<CartComponent> Carts { get; set; } = new HashSet<CartComponent>();

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public LocationComponent Location { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public ProductionComponent Production { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public AssignmentComponent Assignment { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public ExchangeComponent Exchange { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public EntityRegistryComponent EntityRegistry { get; private set; }

        public event EventHandler<EmployeeEventArgs> EmployeeArrived;
        public event EventHandler<EmployeeEventArgs> EmployeeLeft;
        public event EventHandler<CartEventArgs> CartArrived;
        public event EventHandler<CartEventArgs> CartLeft;
        public event EventHandler<HiringEventArgs> EmployeeHired;
        public event EventHandler<HiringEventArgs> CartHired;
        public event EventHandler<CostEventArgs> WagePaid;

        public bool IsEmployed(EmployeeComponent employee) => Employees.Contains(employee);

        public bool IsAvailable(EmployeeComponent employee) => employee && Location.EntityRegistry.IsRegistered(employee.Entity);

        public bool IsEmployed(CartComponent cart) => Carts.Contains(cart);

        public bool IsAvailable(CartComponent cart) => Location.EntityRegistry.IsRegistered(cart.Entity);

        void Awake()
        {
            this.Log("Company created");

            Location = GetComponent<LocationComponent>();
            Production = GetComponent<ProductionComponent>();
            Exchange = GetComponent<ExchangeComponent>();
            Assignment = GetComponent<AssignmentComponent>();

            if (Locator.Time) Locator.Time.WorkingHourTicked += OnWorkingHourTicked;
            Location.EntityArrived += OnEntityArrived;
            Location.EntityLeft += OnEntityLeft;
        }

        [Button]
        public void HireEmployee()
        {
            this.Log("Company is hiring employee");

            var employeeObject = Instantiate(EmployeePrefab);
            var employee = employeeObject.GetComponent<EmployeeComponent>();
            var entity = employeeObject.GetComponent<EntityComponent>();

            Employees.Add(employee);

            employee.SetCompany(this);

            EmployeeHired?.Invoke(this, new HiringEventArgs(entity, HiringCost));
        }

        [Button]
        public void HireCart()
        {
            this.Log("Company is hiring cart");

            var cartObject = Instantiate(CartPrefab);
            var cart = cartObject.GetComponent<CartComponent>();
            var entity = cart.GetComponent<EntityComponent>();

            Carts.Add(cart);

            cart.SetCompany(this);

            CartHired?.Invoke(this, new HiringEventArgs(entity, CartCost));
        }

        void OnEntityArrived(object sender, EntityEventArgs e)
        {
            var employee = e.Entity.GetComponent<EmployeeComponent>();
            var cart = e.Entity.GetComponent<CartComponent>();

            if (IsEmployed(employee))
            {
                this.Log($"Employee {employee} arrived at company");
                EmployeeArrived?.Invoke(this, new EmployeeEventArgs(employee));
            }

            if (IsEmployed(cart))
            {
                this.Log($"Cart {cart} arrived at company");
                CartArrived?.Invoke(this, new CartEventArgs(cart));
            }
        }

        void OnEntityLeft(object sender, EntityEventArgs e)
        {
            var employee = e.Entity.GetComponent<EmployeeComponent>();
            var cart = e.Entity.GetComponent<CartComponent>();

            if (IsEmployed(employee))
            {
                this.Log($"Employee {employee} left company");
                EmployeeLeft?.Invoke(this, new EmployeeEventArgs(employee));
            }

            if (IsEmployed(cart))
            {
                this.Log($"Cart {cart} left company");
                CartLeft?.Invoke(this, new CartEventArgs(cart));
            }
        }

        void OnWorkingHourTicked(object sender, TimeEventArgs e)
        {
            var totalWage = Employees.Count * WagePerHour;

            this.Log($"Company billed wages {totalWage}");
            WagePaid?.Invoke(this, new CostEventArgs(totalWage));
        }
    }
}