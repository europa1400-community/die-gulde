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
        GameObject EmployeePrefab { get; set; }

        [OdinSerialize]
        HashSet<EmployeeComponent> Employees { get; set; } = new HashSet<EmployeeComponent>();

        [OdinSerialize]
        LocationComponent Location { get; set; }

        public event EventHandler<EmployeeEventArgs> Arrived;
        public event EventHandler<EmployeeEventArgs> Left;

        void Awake()
        {
            Location = GetComponent<LocationComponent>();

            Location.EntityRegistry.Registered -= OnEntityRegistered;
            Location.EntityRegistry.Unregistered -= OnEntityUnregistered;

            Location.EntityRegistry.Registered += OnEntityRegistered;
            Location.EntityRegistry.Unregistered += OnEntityUnregistered;
        }

        [Button]
        void Hire()
        {
            var employee = Instantiate(EmployeePrefab);
            var employeeComponent = employee.GetComponent<EmployeeComponent>();
            if (!employeeComponent) return;

            var entityComponent = employee.GetComponent<EntityComponent>();
            if (!entityComponent) return;

            Employees.Add(employeeComponent);

            Location.EntityRegistry.Register(entityComponent);
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

        #region OdinInspector

        void OnValidate()
        {
            Location = GetComponent<LocationComponent>();

            Location.EntityRegistry.Registered -= OnEntityRegistered;
            Location.EntityRegistry.Registered += OnEntityRegistered;
        }

        #endregion
    }
}