using System;
using System.Collections.Generic;
using Gulde.Company;

namespace Gulde.Production
{
    public class ProductionEventArgs : EventArgs
    {
        public ProductionEventArgs(Recipe recipe, List<EmployeeComponent> employees)
        {
            Recipe = recipe;
            Employees = employees;
        }

        public Recipe Recipe { get; }

        public List<EmployeeComponent> Employees { get; }
    }
}