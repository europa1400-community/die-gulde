using System;
using System.Collections.Generic;
using System.Linq;
using Gulde.Company.Employees;
using Gulde.Economy;
using Gulde.Entities;
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
        public float WagePerHour { get; set; }

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
        public AssignmentComponent Assignment { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public ExchangeComponent Exchange { get; set; }

        [OdinSerialize]
        [ReadOnly]
        [FoldoutGroup("Debug")]
        public EntityRegistryComponent EntityRegistry { get; private set; }

        public HashSet<EmployeeComponent> WorkingEmployees =>
            Employees.Where(employee => employee && employee.IsWorking).ToHashSet();

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

            Location.IgnoreTemporarily(entity);

            Carts.Add(cart);

            cart.SetCompany(this);

            CartHired?.Invoke(this, new HiringEventArgs(entity, CartCost));
        }

        void OnEntityArrived(object sender, EntityEventArgs e)
        {
            var employee = e.Entity.GetComponent<EmployeeComponent>();
            var cart = e.Entity.GetComponent<CartComponent>();

            if (IsEmployed(employee)) EmployeeArrived?.Invoke(this, new EmployeeEventArgs(employee));
            if (IsEmployed(cart)) CartArrived?.Invoke(this, new CartEventArgs(cart));
        }

        void OnEntityLeft(object sender, EntityEventArgs e)
        {
            var employee = e.Entity.GetComponent<EmployeeComponent>();
            var cart = e.Entity.GetComponent<CartComponent>();

            if (IsEmployed(employee)) EmployeeLeft?.Invoke(this, new EmployeeEventArgs(employee));
            if (IsEmployed(cart)) CartLeft?.Invoke(this, new CartEventArgs(cart));
        }

        void OnWorkingHourTicked(object sender, TimeEventArgs e)
        {
            var totalWage = WorkingEmployees.Count * WagePerHour;
            WagePaid?.Invoke(this, new CostEventArgs(totalWage));
        }
    }
}