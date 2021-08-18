using System;
using System.Collections.Generic;
using Gulde.Entities;
using Gulde.Maps;
using Gulde.Production;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
        GameObject EmployeePrefab { get; set; }

        [OdinSerialize]
        GameObject CartPrefab { get; set; }

        [OdinSerialize]
        HashSet<EmployeeComponent> Employees { get; set; } = new HashSet<EmployeeComponent>();

        [OdinSerialize]
        HashSet<EntityComponent> Carts { get; set; } = new HashSet<EntityComponent>();

        [OdinSerialize]
        LocationComponent Location { get; set; }

        public event EventHandler<EmployeeEventArgs> Arrived;
        public event EventHandler<EmployeeEventArgs> Left;

        public event EventHandler<HiringEventArgs> EmployeeHired;
        public event EventHandler<HiringEventArgs> CartHired;

        void Awake()
        {
            Location = GetComponent<LocationComponent>();

            Location.EntityRegistry.Registered -= OnEntityRegistered;
            Location.EntityRegistry.Unregistered -= OnEntityUnregistered;

            Location.EntityRegistry.Registered += OnEntityRegistered;
            Location.EntityRegistry.Unregistered += OnEntityUnregistered;
        }

        [Button]
        void HireEmployee()
        {
            var employee = Instantiate(EmployeePrefab);
            var employeeComponent = employee.GetComponent<EmployeeComponent>();
            if (!employeeComponent) return;

            var entityComponent = employee.GetComponent<EntityComponent>();
            if (!entityComponent) return;

            Employees.Add(employeeComponent);

            Location.Map.EntityRegistry.Register(entityComponent);
            Location.EntityRegistry.Register(entityComponent);

            EmployeeHired?.Invoke(this, new HiringEventArgs(entityComponent, HiringCost));
        }

        [Button]
        void HireCart()
        {
            var cart = Instantiate(CartPrefab);

            var entityComponent = cart.GetComponent<EntityComponent>();
            if (!entityComponent) return;

            Carts.Add(entityComponent);

            Location.Map.EntityRegistry.Register(entityComponent);
            Location.EntityRegistry.Register(entityComponent);

            CartHired?.Invoke(this, new HiringEventArgs(entityComponent, CartCost));
        }

        void OnEntityRegistered(object sender, EntityEventArgs e)
        {
            var employeeComponent = e.Entity.GetComponent<EmployeeComponent>();
            if (!employeeComponent) return;
            if (!Employees.Contains(employeeComponent)) return;

            Arrived?.Invoke(this, new EmployeeEventArgs(employeeComponent));
        }

        void OnEntityUnregistered(object sender, EntityEventArgs e)
        {
            var employeeComponent = e.Entity.GetComponent<EmployeeComponent>();
            if (!employeeComponent) return;
            if (!Employees.Contains(employeeComponent)) return;

            Left?.Invoke(this, new EmployeeEventArgs(employeeComponent));
        }
    }
}