using System.Collections;
using GuldeLib.Company;
using GuldeLib.Company.Employees;
using MonoLogger.Runtime;
using UnityEngine;

namespace GuldeLib.Builders
{
    // TODO: Add documentation
    public class EmployeeBuilder : Builder
    {
        [LoadAsset("prefab_employee")]
        GameObject EmployeePrefab { get; set; }

        public GameObject EmployeeObject { get; private set; }

        public EmployeeComponent Employee { get; private set; }

        CompanyComponent Company { get; set; }

        public EmployeeBuilder WithCompany(CompanyComponent company)
        {
            Company = company;
            return this;
        }

        public override IEnumerator Build()
        {
            if (!Company)
            {
                this.Log($"Cannot create employee without a company.", LogType.Error);
                yield break;
            }

            yield return base.Build();

            EmployeeObject = Object.Instantiate(EmployeePrefab);

            Employee = EmployeeObject.GetComponent<EmployeeComponent>();
            Employee.SetCompany(Company);
        }
    }
}