using System;
using System.Collections.Generic;
using Gulde.Company;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using Sirenix.Utilities;

namespace Gulde.Production
{
    public class AssignmentComponent : SerializedMonoBehaviour
    {
        [OdinSerialize]
        [BoxGroup("Info")]
        Dictionary<EmployeeComponent, Recipe> Assignments { get; set; } = new Dictionary<EmployeeComponent, Recipe>();

        [OdinSerialize]
        [FoldoutGroup("Debug")]
        ProductionRegistryComponent Registry { get; set; }

        [OdinSerialize]
        [FoldoutGroup("Debug")]
        CompanyComponent Company { get; set; }

        public event EventHandler<AssignmentEventArgs> Assigned;
        public event EventHandler<AssignmentEventArgs> Unassigned;

        public bool IsAssigned(EmployeeComponent employee) =>
            Assignments.ContainsKey(employee) && Assignments[employee];

        public bool IsAssigned(Recipe recipe) =>
            Assignments.ContainsValue(recipe);

        public bool IsAssignable(EmployeeComponent employee) =>
            Company.IsEmployed(employee) && Company.IsAvailable(employee) && (!Assignments[employee] || !Assignments[employee].IsExternal);

        public int AssignmentCount(Recipe recipe) =>
            Assignments.Count(pair => pair.Value == recipe);

        public int AssignmentCount() =>
            Assignments.Count;

        public List<EmployeeComponent> GetAssignedEmployees(Recipe recipe) =>
            Assignments.Keys.Where(employee => Assignments[employee] == recipe).ToList();

        public Recipe GetRecipe(EmployeeComponent employee) =>
            IsAssigned(employee) ? Assignments[employee] : null;

        public HashSet<Recipe> GetAssignedRecipes => Assignments.Values.ToHashSet();

        void Awake()
        {
            Registry = GetComponent<ProductionRegistryComponent>();
            Company = GetComponent<CompanyComponent>();
        }

        void RegisterEmployee(EmployeeComponent employee)
        {
            if (!Assignments.ContainsKey(employee)) Assignments.Add(employee, null);
        }

        public void Assign(EmployeeComponent employee, Recipe recipe)
        {
            if (!employee) return;
            if (!recipe) return;
            if (!Company.IsEmployed(employee)) return;
            RegisterEmployee(employee);

            if (!IsAssignable(employee)) return;

            Unassign(employee);
            Assignments[employee] = recipe;

            Assigned?.Invoke(this, new AssignmentEventArgs(employee, recipe));
        }

        public void AssignAll(Recipe recipe)
        {
            foreach (var employee in Company.Employees)
            {
                Assign(employee, recipe);
            }
        }

        public void Unassign(EmployeeComponent employee)
        {
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
            foreach (var employee in Company.Employees)
            {
                Unassign(employee);
            }
        }
    }
}