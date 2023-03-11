using System;
using UnityEngine;

namespace Gulde.Core
{
    [Serializable]
    public class Assignment
    {
        [SerializeField]
        public Recipe recipe;
        
        [SerializeField]
        public EmployeeComponent employeeComponent;
    }
}