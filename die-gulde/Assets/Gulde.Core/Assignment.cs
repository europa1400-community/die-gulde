using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gulde.Core
{
    [Serializable]
    public class Assignment
    {
        [SerializeField]
        public Recipe recipe;
        
        [FormerlySerializedAs("employeeComponent")]
        [SerializeField]
        public EmployeeComponent employee;
    }
}