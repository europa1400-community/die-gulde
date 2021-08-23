using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Company.Employees;
using Gulde.Economy;
using Gulde.Entities;
using Gulde.Extensions;
using Gulde.Maps;
using Gulde.Production;
using Gulde.Timing;
using Gulde.Vehicles;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Gulde.Company
{
    [HideMonoScript]
    [RequireComponent(typeof(LocationComponent))]
    public class CompanyComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Settings")]
        int HiringCost { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        int CartCost { get; set; }

        [OdinSerialize]
        [BoxGroup("Settings")]
        float WagePerHour { get; set; }

        [OdinSerialize]
        [Required]
        [BoxGroup("Info")]
        public WealthComponent Owner { get; set; }

        [OdinSerialize]
        GameObject EmployeePrefab { get; set; }

        [OdinSerialize]
        GameObject CartPrefab { get; set; }

        [OdinSerialize]
        public HashSet<EmployeeComponent> Employees { get; set; } = new HashSet<EmployeeComponent>();

        [OdinSerialize]
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
        public ExchangeComponent Exchange { get; set; }

        HashSet<EmployeeComponent> WorkingEmployees =>
            Employees.Where(e => Location.EntityRegistry.IsRegistered(e.Entity)).ToHashSet();

        public event EventHandler<EmployeeEventArgs> EmployeeArrived;
        public event EventHandler<EmployeeEventArgs> EmployeeLeft;

        public event EventHandler<HiringEventArgs> EmployeeHired;
        public event EventHandler<HiringEventArgs> CartHired;

        public event EventHandler<CostEventArgs> WagePaid;

        public bool IsEmployed(EmployeeComponent employee) => Employees.Contains(employee);

        public bool IsAvailable(EmployeeComponent employee) => Location.EntityRegistry.IsRegistered(employee.Entity);

        public bool IsEmployed(CartComponent cart) => Carts.Contains(cart);

        public bool IsAvailable(CartComponent cart) => Location.EntityRegistry.IsRegistered(cart.Entity);

        void Awake()
        {
            Location = GetComponent<LocationComponent>();
            Production = GetComponent<ProductionComponent>();
            Exchange = GetComponent<ExchangeComponent>();

            if (Locator.Time) Locator.Time.WorkingHourTicked += OnWorkingHourTicked;
            Location.EntityRegistry.Registered += OnEntityRegistered;
            Location.EntityRegistry.Unregistered += OnEntityUnregistered;
        }

        [Button]
        public void HireEmployee()
        {
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
            var cartObject = Instantiate(CartPrefab);
            var cart = cartObject.GetComponent<CartComponent>();
            var entity = cart.GetComponent<EntityComponent>();

            Carts.Add(cart);

            cart.SetCompany(this);

            CartHired?.Invoke(this, new HiringEventArgs(entity, CartCost));
        }

        void OnEntityRegistered(object sender, EntityEventArgs e)
        {
            var employeeComponent = e.Entity.GetComponent<EmployeeComponent>();
            if (!employeeComponent) return;
            if (!Employees.Contains(employeeComponent)) return;

            EmployeeArrived?.Invoke(this, new EmployeeEventArgs(employeeComponent));
        }

        void OnEntityUnregistered(object sender, EntityEventArgs e)
        {
            var employeeComponent = e.Entity.GetComponent<EmployeeComponent>();
            if (!employeeComponent) return;
            if (!Employees.Contains(employeeComponent)) return;

            EmployeeLeft?.Invoke(this, new EmployeeEventArgs(employeeComponent));
        }

        void OnWorkingHourTicked(object sender, TimeEventArgs e)
        {
            var totalWage = WorkingEmployees.Count * WagePerHour;
            WagePaid?.Invoke(this, new CostEventArgs(totalWage));
        }
    }
}