using System;
using System.Collections.Generic;
using System.Linq;
using GuldeLib.Company;
using GuldeLib.Company.Employees;
using MonoLogger.Runtime;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace GuldeLib.Production
{
    public class AssignmentComponent : SerializedMonoBehaviour
    {
        [ShowInInspector]
        [BoxGroup("Info")]
        public Dictionary<EmployeeComponent, Recipe> Assignments { get; } = new Dictionary<EmployeeComponent, Recipe>();

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        ProductionRegistryComponent Registry { get; set; }

        [ShowInInspector]
        [FoldoutGroup("Debug")]
        CompanyComponent Company { get; set; }

        public event EventHandler<AssignmentEventArgs> Assigned;
        public event EventHandler<AssignmentEventArgs> Unassigned;

        public bool IsAssigned(EmployeeComponent employee) =>
            Assignments.ContainsKey(employee) && Assignments[employee];

        public bool IsAssigned(Recipe recipe) =>
            Assignments.ContainsValue(recipe);

        public bool IsAssignedExternally(EmployeeComponent employee) =>
            IsAssigned(employee) && Assignments[employee].IsExternal;

        public bool IsAssignable(EmployeeComponent employee) =>
            Company.IsEmployed(employee) && Company.IsAvailable(employee) && !IsAssignedExternally(employee);

        public int AssignmentCount(Recipe recipe) =>
            Assignments.Count(pair => pair.Value == recipe);

        public int AssignmentCount() =>
            Assignments.Count;

        public List<EmployeeComponent> GetAssignedEmployees(Recipe recipe) => recipe
                ? Assignments.Where(pair => pair.Value == recipe).Select(pair => pair.Key).ToList()
                : new List<EmployeeComponent>();

        public Recipe GetRecipe(EmployeeComponent employee) =>
            IsAssigned(employee) ? Assignments[employee] : null;

        public HashSet<Recipe> AssignedRecipes => Assignments
            .Values
            .Where(e => e)
            .ToHashSet();

        void Awake()
        {
            this.Log("Assignment initializing");

            Registry = GetComponent<ProductionRegistryComponent>();
            Company = GetComponent<CompanyComponent>();
        }

        void RegisterEmployee(EmployeeComponent employee)
        {
            this.Log($"Assignment registering {employee}");

            if (!Assignments.ContainsKey(employee)) Assignments.Add(employee, null);
        }

        public void Assign(EmployeeComponent employee, Recipe recipe)
        {
            this.Log($"Assignment assigning {employee} to {recipe}");

            if (!employee)
            {
                this.Log($"Assignment can't assign: Employee was null", LogType.Warning);
                return;
            }

            if (!recipe)
            {
                this.Log($"Assignment can't assign: Recipe was null", LogType.Warning);
                return;
            }

            if (!Company.IsEmployed(employee))
            {
                this.Log($"Assignment can't assign: Employee was not employed", LogType.Warning);
                return;
            }

            RegisterEmployee(employee);

            if (!IsAssignable(employee))
            {
                this.Log($"Assignment can't assign: Employee was not assignable", LogType.Warning);
                return;
            }

            Unassign(employee);
            Assignments[employee] = recipe;

            Assigned?.Invoke(this, new AssignmentEventArgs(employee, recipe));
        }

        public void AssignAll(Recipe recipe)
        {
            this.Log($"Assignment assigning all employees to {recipe}");

            foreach (var employee in Company.Employees)
            {
                Assign(employee, recipe);
            }
        }

        public void Unassign(EmployeeComponent employee)
        {
            this.Log($"Assignment unassigning {employee}");

            if (!employee) return;
            if (!Company.IsEmployed(employee)) return;
            if (!IsAssigned(employee)) return;

            var recipe = Assignments[employee];
            if (recipe.IsExternal && Registry.IsProducing(recipe)) return;

            Assignments.Remove(employee);

            Unassigned?.Invoke(this, new AssignmentEventArgs(employee, recipe));
        }

        public void UnassignAll()
        {
            this.Log("Assignment unassigning all employees");

            foreach (var employee in Company.Employees)
            {
                Unassign(employee);
            }
        }
    }
}